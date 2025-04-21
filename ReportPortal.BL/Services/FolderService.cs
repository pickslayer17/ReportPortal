using AutoMapper;
using ReportPortal.BL.Constatnts;
using ReportPortal.BL.Models;
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
        private readonly IMapper _mapper;

        public FolderService(IRunRepository runRepository, IFolderRepository folderRepository, IMapper mapper)
        {
            _runRepository = runRepository;
            _folderRepository = folderRepository;
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
            var run = await _runRepository.GetByAsync(r => r.Id == runId, cancellationToken);
            if (run == null) throw new DirectoryNotFoundException($"There is no run with such id {runId}!");

            var folderNames = path.ToLower().Split('.');
            if (folderNames.Length == 0) throw new DirectoryNotFoundException($"Test cannot be added without directory.");

            Folder rootFolder;
            rootFolder = run.Folders.FirstOrDefault(f => f.Name == FolderNames.RootFolderName);
            if (rootFolder == null)
            {
                throw new Exception($"No root folder for run id {run.Id}");
            }

            return await GetIdOrAddFolderAsync(rootFolder, run.Id, 1, folderNames, cancellationToken);
        }

        private async Task<int> GetIdOrAddFolderAsync(Folder parentFolder, int runId, int folderLevel, string[] folderNames, CancellationToken cancellationToken = default)
        {
            var currentFolderName = folderNames[0];

            if (folderNames.Length == 1)
            {
                if (parentFolder.Children != null && parentFolder.Children.Any(c => c.Name == currentFolderName))
                {
                    return parentFolder.Children.First(c => c.Name == currentFolderName).Id;
                }
                else
                {
                    var newFolder = await CreateFolderAsync(parentFolder, runId, currentFolderName, folderLevel, cancellationToken);

                    return newFolder.Id;
                }
            }
            else
            {
                Folder childFolder = null;
                if (parentFolder.Children != null && parentFolder.Children.Any(c => c.Name == currentFolderName))
                {
                    childFolder = parentFolder.Children.First(c => c.Name == currentFolderName);
                }
                else
                {
                    childFolder = await CreateFolderAsync(parentFolder, runId, currentFolderName, folderLevel, cancellationToken);
                }

                folderNames = folderNames.Skip(1).ToArray();

                return await GetIdOrAddFolderAsync(childFolder, runId, ++folderLevel, folderNames, cancellationToken);
            }
        }

        public async Task<int> CreateRootFolderAsync(int runId, CancellationToken cancellationToken = default)
        {
            bool exists = await _folderRepository.ExistsAsync(f => f.Name == FolderNames.RootFolderName && f.RunId == runId, cancellationToken);

            if (!exists)
            {
                var rootFolder = await CreateFolderAsync(null, runId, FolderNames.RootFolderName, 0, cancellationToken);

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

            foreach (var child in folder.Children)
            {
                await DeleteFolderAsync(child.Id);
            }

            await _folderRepository.RemoveByIdAsync(folderId, cancellationToken);
        }
    }
}
