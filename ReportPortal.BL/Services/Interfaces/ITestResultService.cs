using ReportPortal.BL.Models;

namespace ReportPortal.BL.Services.Interfaces
{
    public interface ITestResultService : IServiceBase<TestResultDto>
    {
        public Task<int> AddTestResultToTestAsync(int testId, TestResultDto testDto, CancellationToken cancellationToken = default);
        public Task<IEnumerable<TestResultDto>> GetTestTestResultsAsync(int testId, CancellationToken cancellationToken = default);
    }
}
