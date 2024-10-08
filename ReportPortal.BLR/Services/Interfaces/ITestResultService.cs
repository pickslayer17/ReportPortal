using ReportPortal.BL.Models;
using ReportPortal.BL.Models.Created;

namespace ReportPortal.BL.Services.Interfaces
{
    public interface ITestResultService : IServiceBase<TestResultDto, TestResultCreatedDto>
    {
        public Task<int> AddTestResultToTestAsync(int testId, TestResultDto testDto, CancellationToken cancellationToken = default);
        public Task<IEnumerable<TestResultDto>> GetTestTestResultsAsync(int testId, CancellationToken cancellationToken = default);
    }
}
