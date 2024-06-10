using ReportPortal.BL.Models;
using ReportPortal.BL.Models.Created;

namespace ReportPortal.BL.Services.Interfaces
{
    public interface ITestResultService : IServiceBase<TestResultDto, TestResultCreatedDto>
    {
        public Task<int> AddTestResultToTest(int testId, TestResultDto testDto, CancellationToken cancellationToken = default);
    }
}
