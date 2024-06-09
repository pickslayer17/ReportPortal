using AutoMapper;
using ReportPortal.BL.Constatnts;
using ReportPortal.BL.Models;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.DAL.Models.RunProjectManagement;
using ReportPortal.DAL.Repositories.Interfaces;
using System;
using System.Linq.Expressions;

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

        public async Task<int> GetIdOrAddFolderInRun(int runId, string path)
        {
            var run = await _runRepository.GetByAsync(r => r.Id == runId);
            if (run == null) throw new DirectoryNotFoundException($"There is no run with such id {runId}!");

            var folderNames = path.Split('.');
            if (folderNames.Length == 0) throw new DirectoryNotFoundException($"Test cannot be added without directory.");

            var rootFolder = await _folderRepository.GetByAsync(f => f.Id == run.RootFolderId);

            return await GetIdOrAddFolder(rootFolder, folderNames, runId);
        }

        private async Task<int> GetIdOrAddFolder(FolderRunItem parentFolder, string[] folderNames, int runId)
        {
            var parentFolderChildNames = new List<string>();
            if (parentFolder.ChildFolderIds != null)
            {
                var parentFolderChildIds = parentFolder.ChildFolderIds.ToArray();
                /// verify if parentFolderChildNames contain folderNames[0]
                var parentFolderChilds = await _folderRepository.GetAllByAsync(f => parentFolderChildIds.Contains(f.Id));
                parentFolderChildNames = parentFolderChilds.Select(f => f.Name).ToList();
            }

            /// if no -> crate folder,  
            if (!parentFolderChildNames.Contains(folderNames[0]))
            {
                var newFolderId = await CreateFolder(parentFolder.Id, folderNames[0], runId);
                var newFolder = await _folderRepository.GetByAsync(f => f.Id == newFolderId);

                if (folderNames.Length == 1)
                {
                    return newFolder.Id;
                }
                else
                {
                    return await GetIdOrAddFolder(newFolder, folderNames.Skip(1).ToArray(), runId);
                }
            }
            /// if yes -> choose folder, call GetIdOrAddFolder() again
            else
            {
                var foundFolder = await _folderRepository.GetByAsync(f => f.Name == folderNames[0] && parentFolder.ChildFolderIds.Contains(f.Id));
                if (folderNames.Length == 1)
                {
                    return foundFolder.Id;
                }
                else
                {
                    return await GetIdOrAddFolder(foundFolder, folderNames.Skip(1).ToArray(), runId);
                }
            }
        }

        private async Task<int> CreateFolder(int? folderParentId, string folderName, int runId)
        {
            var folderRunItem = new FolderRunItem
            {
                Name = folderName,
                ParentId = folderParentId,
                RunId = runId
            };

            int folderId = await _folderRepository.InsertAsync(folderRunItem);
            var parentFolder = await _folderRepository.GetByAsync(f => f.Id == folderParentId);
            if (parentFolder.ChildFolderIds == null)
            {
                parentFolder.ChildFolderIds = new List<int> { folderId };
            }
            else
            {
                parentFolder.ChildFolderIds.Add(folderId);
            }

            await _folderRepository.UpdateItem(parentFolder);

            return folderId;
        }


        public async Task AttachTestToFolder(int folderId, int testId)
        {
            var folder = await _folderRepository.GetByAsync(f => f.Id == folderId);
            if (folder == null) throw new DirectoryNotFoundException($"There is no folder with such id {folderId}!");

            if (folder.TestIds == null)
            {
                folder.TestIds = new List<int> { testId };
            }
            else
            {
                folder.TestIds.Add(testId);
            }

            await _folderRepository.UpdateItem(folder);
        }

        public async Task<FolderDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var folderRunItem = await _folderRepository.GetByAsync(f => f.Id == id, cancellationToken);
            return _mapper.Map<FolderDto>(folderRunItem);
        }

        public async Task<IEnumerable<FolderDto>> GetAllRunChildAsync(int runId, CancellationToken cancellationToken = default)
        {

            var rootFolder = await _folderRepository.GetByAsync(f => f.Name == FolderNames.RootFolderName && f.Id == runId, cancellationToken);
            var folders = new List<FolderRunItem>();
            foreach (var childId in rootFolder.ChildFolderIds)
            {
                var childFolder = await _folderRepository.GetByAsync(f => f.Id == childId, cancellationToken);
                if(childFolder == null) continue;
                folders.Add(childFolder);
            }

            return folders.Select(f => _mapper.Map<FolderDto>(f));
        }
    }
}
