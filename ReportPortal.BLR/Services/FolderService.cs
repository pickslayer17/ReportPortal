using ReportPortal.BL.Models;
using ReportPortal.BL.Models.Created;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.DAL.Models.RunProjectManagement;
using ReportPortal.DAL.Repositories.Interfaces;
using System.Linq;
using System.Linq.Expressions;

namespace ReportPortal.BL.Services
{
    public class FolderService : IFolderService
    {
        private readonly IFolderRepository _folderRepository;
        private readonly IRunRepository _runRepository;

        public FolderService(IRunRepository runRepository, IFolderRepository folderRepository)
        {
            _runRepository = runRepository;
            _folderRepository = folderRepository;
        }

        public async Task<int> GetIdOrAddFolderInRun(int runId, string path)
        {
            var run = await _runRepository.GetByAsync(r => r.Id == runId);
            if (run == null) throw new DirectoryNotFoundException($"There is no run with such id {runId}!");

            var folderNames = path.Split('.');
            if (folderNames.Length == 0) throw new DirectoryNotFoundException($"Test cannot be added without directory.");

            var rootFolder = await _folderRepository.GetByAsync(f => f.Id == run.RootFolderId);

            return await GetIdOrAddFolder(rootFolder, folderNames);
        }

        private async Task<int> GetIdOrAddFolder(FolderRunItem parentFolder, string[] folderNames)
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
                var newFolderId = await CreateFolder(parentFolder.Id, folderNames[0]);
                var newFolder = await _folderRepository.GetByAsync(f => f.Id == newFolderId);

                if(folderNames.Length == 1)
                {
                    return newFolder.Id;
                }
                else
                {
                    return await GetIdOrAddFolder(newFolder, folderNames.Skip(1).ToArray());
                }
            }
            /// if yes -> choose folder, call GetIdOrAddFolder() again
            else
            {
                var foundFolder = await _folderRepository.GetByAsync(f => f.Name == folderNames[0] && parentFolder.ChildFolderIds.Contains(f.Id));

                return await GetIdOrAddFolder(foundFolder, folderNames.Skip(1).ToArray());
            }
        }

        private async Task<int> CreateFolder(int? folderParentId, string folderName)
        {
            var folderRunItem = new FolderRunItem
            {
                Name = folderName,
                ParentId = folderParentId,
            };

            int folderId = await _folderRepository.InsertAsync(folderRunItem);
            var parentFolder = await _folderRepository.GetByAsync(f => f.Id == folderParentId);
            if(parentFolder.ChildFolderIds == null)
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

            if(folder.TestIds == null)
            {
                folder.TestIds = new List<int> { testId  };
            }
            else
            {
                folder.TestIds.Add(testId);
            }

            await _folderRepository.UpdateItem(folder);
        }

        private async Task<FolderRunItem> GetByAsync(Expression<Func<FolderRunItem, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _folderRepository.GetByAsync(predicate, cancellationToken);
        }

        public Task<FolderCreatedDto> CreateAsync(FolderDto projectForCreationDto, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FolderRunItem>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FolderRunItem>> GetAllByAsync(Expression<Func<FolderRunItem, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
