using ReportPortal.BL.Models.Created;
using ReportPortal.BL.Models.ForCreation;
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
            var rootFolderChildIds = rootFolder.ChildFolderIds.ToArray();

            return await GetIdOrAddFolder(rootFolderChildIds, folderNames);
        }

        private async Task<int> GetIdOrAddFolder(int[] childFolderIds, string[] folderNames)
        {
            var folderNameToAdd = folderNames[0];
            var folderNamesToLookIn = await _folderRepository.GetAllByAsync(f => f.Name == folderNameToAdd);
            if(folderNamesToLookIn.Any(fn => fn.Equals(folderNameToAdd)))
            {
                var existingFolder = folderNamesToLookIn.First(f => f.Name == folderNameToAdd);
                if (folderNames.Count() == 1)
                {
                    return existingFolder.Id;
                }
                else
                {
                    return await GetIdOrAddFolder(existingFolder.ChildFolderIds.ToArray(), folderNames.Skip(1).ToArray());
                }
            }
            else
            {
               
            }

            return 0;
        }

        private async Task<int> CreateFolder(int? folderParentId, string folderName)
        {
            var folderRunItem = new FolderRunItem
            {
                Name = folderName,
                ParentId = folderParentId,
            };

            int folderId = await _folderRepository.InsertAsync(folderRunItem);

            if (folderParentId.HasValue)
            {
                var parentFolder = await _folderRepository.GetByAsync(f => f.Id == folderParentId);
                parentFolder.ChildFolderIds.Add(folderId);
                await _folderRepository.UpdateItem(parentFolder);
            }

            return folderId;
        }

        private async Task<FolderRunItem> GetByAsync(Expression<Func<FolderRunItem, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _folderRepository.GetByAsync(predicate, cancellationToken);
        }

        public Task<FolderCreatedDto> CreateAsync(FolderForCreationDto projectForCreationDto, CancellationToken cancellationToken = default)
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
