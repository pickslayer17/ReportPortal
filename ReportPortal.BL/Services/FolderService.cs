using AutoMapper;
using ReportPortal.BL.Constatnts;
using ReportPortal.BL.Models;
using ReportPortal.BL.Services.Caching;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.DAL.Exceptions;
using ReportPortal.DAL.Models.RunProjectManagement;
using ReportPortal.DAL.Repositories.Interfaces;

namespace ReportPortal.BL.Services
{
    public class FolderService : IFolderService
    {
        private readonly IFolderRepository _folderRepository;
        private readonly IRunRepository _runRepository;
        private readonly IFolderTreeCache _folderTreeCache;
        private readonly IMapper _mapper;

        public FolderService(IRunRepository runRepository, IFolderRepository folderRepository, IFolderTreeCache folderTreeCache, IMapper mapper)
        {
            _runRepository = runRepository;
            _folderRepository = folderRepository;
            _folderTreeCache = folderTreeCache;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FolderDto>> GetAllFoldersAsync(int runId, CancellationToken cancellationToken = default)
        {
            var folders = await _folderRepository.GetAllByAsync(f => f.RunId == runId, cancellationToken);
            var folderDto = folders.Select(f => _mapper.Map<FolderDto>(f));

            return folderDto;
        }

        public async Task<int> DoesFolderExistsAsync(int runId, string path, CancellationToken cancellationToken = default)
        {
            var run = await _runRepository.GetByAsync(r => r.Id == runId, cancellationToken);
            if (run == null) throw new DirectoryNotFoundException($"There is no run with such id {runId}!");

            var folderNames = path.ToLower().Split('.');
            if (folderNames.Length == 0) throw new DirectoryNotFoundException($"Test cannot be added without directory.");

            Folder rootFolder;
            rootFolder = run.Folders.FirstOrDefault(f => f.FolderLevel == 0);
            if (rootFolder == null)
            {
                throw new Exception($"Critical app error - no root folder for the run with id {runId}");
            }

            var folder = await GetFolderAsync(rootFolder, folderNames, cancellationToken);

            return folder.Id;
        }

        private async Task<Folder> GetFolderAsync(Folder parentFolder, string[] folderNames, CancellationToken cancellationToken = default)
        {
            var currentFolderName = folderNames[0];
            var currentFolder = parentFolder.Children.FirstOrDefault(f => f.Name.ToLower() == currentFolderName);
            if (currentFolder == null)
                throw new FolderNotFoundException(
                    $"Folder with name {currentFolderName} was not found in parent folder {parentFolder.Name} id: {parentFolder.Id}");

            if (folderNames.Length == 1)
            {
                return currentFolder;
            }
            else
            {
                return await GetFolderAsync(currentFolder, folderNames.Skip(1).ToArray(), cancellationToken);
            }
        }

        public async Task<int> GetIdOrAddFolderInRunAsync(int runId, string path, CancellationToken cancellationToken = default)
        {
            var map = await GetIdOrAddFoldersInRunAsync(runId, new[] { path }, cancellationToken);
            return map[path];
        }

        /// <summary>
        /// Resolves (creating when missing) the folder id for every path in one pass.
        /// Backed by the per-run in-memory tree cache: existing paths are O(1) lookups,
        /// missing folders are inserted in a single batch and written through to the cache.
        /// </summary>
        public async Task<Dictionary<string, int>> GetIdOrAddFoldersInRunAsync(int runId, IReadOnlyCollection<string> paths, CancellationToken cancellationToken = default)
        {
            var tree = await EnsureTreeAsync(runId, cancellationToken);
            var result = new Dictionary<string, int>();
            var missing = new List<string>();

            // Fast path: anything already in the cache needs no lock and no DB hit.
            foreach (var path in paths)
            {
                if (result.ContainsKey(path)) continue;
                if (tree.TryResolve(path.ToLower(), out var id)) result[path] = id;
                else missing.Add(path);
            }

            if (missing.Count == 0) return result;

            await tree.Gate.WaitAsync(cancellationToken);
            try
            {
                var newFolders = new List<Folder>();
                var pendingByPath = new Dictionary<string, Folder>(StringComparer.Ordinal);

                foreach (var path in missing)
                {
                    var segments = path.ToLower().Split('.', StringSplitOptions.RemoveEmptyEntries);
                    var prefix = string.Empty;
                    var parentId = tree.RootId;
                    Folder parentPending = null;
                    var level = 1;

                    foreach (var seg in segments)
                    {
                        var childPrefix = prefix.Length == 0 ? seg : prefix + "." + seg;

                        if (tree.TryResolve(childPrefix, out var existingId))
                        {
                            parentId = existingId;
                            parentPending = null;
                        }
                        else if (pendingByPath.TryGetValue(childPrefix, out var pending))
                        {
                            parentPending = pending;
                        }
                        else
                        {
                            var newFolder = new Folder { Name = seg, RunId = runId, FolderLevel = level };
                            if (parentPending != null) newFolder.Parent = parentPending;
                            else newFolder.ParentId = parentId;

                            newFolders.Add(newFolder);
                            pendingByPath[childPrefix] = newFolder;
                            parentPending = newFolder;
                        }

                        prefix = childPrefix;
                        level++;
                    }
                }

                if (newFolders.Count > 0)
                {
                    // One round-trip; EF orders inserts by the Parent navigation and fills the ids.
                    await _folderRepository.InsertRangeAsync(newFolders, cancellationToken);
                    foreach (var pending in pendingByPath)
                        tree.Set(pending.Key, pending.Value.Id);
                }

                foreach (var path in paths)
                {
                    if (result.ContainsKey(path)) continue;
                    if (tree.TryResolve(path.ToLower(), out var id)) result[path] = id;
                }
            }
            finally
            {
                tree.Gate.Release();
            }

            return result;
        }

        private async Task<RunFolderTree> EnsureTreeAsync(int runId, CancellationToken cancellationToken)
        {
            if (_folderTreeCache.TryGet(runId, out var cached)) return cached;

            var folders = await _folderRepository.GetByRunAsync(runId, cancellationToken);
            var root = folders.FirstOrDefault(f => f.ParentId == null && f.Name == FolderNames.RootFolderName)
                       ?? folders.FirstOrDefault(f => f.FolderLevel == 0);
            if (root == null) throw new Exception($"No root folder for run id {runId}");

            var childrenByParent = folders.Where(f => f.ParentId != null).ToLookup(f => f.ParentId.Value);
            var idByPath = new Dictionary<string, int>(StringComparer.Ordinal) { [string.Empty] = root.Id };

            void Walk(int parentId, string parentPath)
            {
                foreach (var child in childrenByParent[parentId])
                {
                    var childPath = parentPath.Length == 0 ? child.Name.ToLower() : parentPath + "." + child.Name.ToLower();
                    idByPath[childPath] = child.Id;
                    Walk(child.Id, childPath);
                }
            }
            Walk(root.Id, string.Empty);

            // GetOrAdd guards against a concurrent loader winning the race.
            return _folderTreeCache.GetOrAdd(runId, new RunFolderTree(root.Id, idByPath));
        }

        public async Task<int> CreateRootFolderAsync(int runId, CancellationToken cancellationToken = default)
        {
            bool exists = await _folderRepository.ExistsAsync(f => f.Name == FolderNames.RootFolderName && f.RunId == runId, cancellationToken);

            if (!exists)
            {
                var rootFolder = await CreateFolderAsync(null, runId, FolderNames.RootFolderName, 0, cancellationToken);

                // New run -> make sure no stale tree lingers; it will load fresh on first use.
                _folderTreeCache.Invalidate(runId);

                return rootFolder.Id;
            }
            else
            {
                throw new Exception($"Run id {runId} already has a root folder");
            }
        }

        private async Task<Folder> CreateFolderAsync(Folder parentFolder, int runId, string folderName, int folderLevel, CancellationToken cancellationToken = default)
        {
            var folder = new Folder
            {
                Name = folderName,
                RunId = runId,
                FolderLevel = folderLevel,
                Parent = parentFolder
            };

            await _folderRepository.InsertAsync(folder, cancellationToken);

            return folder;
        }

        public async Task<FolderDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var folderRunItem = await _folderRepository.GetByAsync(f => f.Id == id, cancellationToken);
            if (folderRunItem == null) throw new FolderNotFoundException($"folder with id {id} was not found");

            return _mapper.Map<FolderDto>(folderRunItem);
        }

        public async Task DeleteFolderAsync(int folderId, CancellationToken cancellationToken = default)
        {
            var folder = await GetByIdAsync(folderId, cancellationToken);

            if (folder.Children != null)
            {
                foreach (var child in folder.Children)
                {
                    await DeleteFolderAsync(child.Id, cancellationToken);
                }
            }

            await _folderRepository.RemoveByIdAsync(folderId, cancellationToken);

            // Folder tree changed -> drop the cached tree so it reloads fresh on next access.
            _folderTreeCache.Invalidate(folder.RunId);
        }
    }
}
