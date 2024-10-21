using ReportPortal.BL.Models;

namespace ReportPortal.BL.Services.Interfaces
{
    public interface ITestReviewService
    {
        public Task UpdateTestReviewAsync(TestReviewDto testDto, CancellationToken cancellationToken = default);
        public Task<TestReviewDto> GetTestReviewAsync(int testId, CancellationToken cancellationToken = default);
    }
}
