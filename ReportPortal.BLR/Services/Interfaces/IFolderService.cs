using ReportPortal.BL.Models;

namespace ReportPortal.BL.Services.Interfaces
{
    public interface IFolderService
    {
        public Task<int> GetIdOrAddFolderInRun(int runId, string path);
        public Task AttachTestToFolder(int folderId, int testId);
        public Task<FolderDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        public Task<IEnumerable<FolderDto>> GetAllRunChildAsync(int runId, CancellationToken cancellationToken = default);
    }
}
