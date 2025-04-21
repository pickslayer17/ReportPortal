using ReportPortal.BL.Models;

namespace ReportPortal.BL.Services.Interfaces
{
    public interface ITestService : IServiceBase<TestDto>
    {
        public Task<TestDto> CreateAsync(TestDto projectForCreationDto, int folderId, CancellationToken cancellationToken = default);
        public Task<IEnumerable<TestDto>> GetAllByFolderIdAsync(int folderId, CancellationToken cancellationToken = default);
        public Task<IEnumerable<TestDto>> GetAllByRunIdAsync(int runId, CancellationToken cancellationToken = default);
        public Task<int> GetTestIdAsync(int folderId, string testName, CancellationToken cancellationToken = default);
    }
}
