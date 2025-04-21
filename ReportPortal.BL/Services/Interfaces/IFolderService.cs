using ReportPortal.BL.Models;

namespace ReportPortal.BL.Services.Interfaces
{
    public interface IFolderService
    {
        public Task<int> GetIdOrAddFolderInRunAsync(int runId, string path, CancellationToken cancellationToken = default);
        public Task<FolderDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        public Task<IEnumerable<FolderDto>> GetAllFoldersAsync(int runId, CancellationToken cancellationToken = default);
        public Task DeleteFolderAsync(int folderId, CancellationToken cancellationToken = default);
        public Task<int> DoesFolderExistsAsync(int runId, string path, CancellationToken cancellationToken = default);
        public Task CreateRootFolderAsync(int runId, CancellationToken cancellationToken = default);
    }
}
