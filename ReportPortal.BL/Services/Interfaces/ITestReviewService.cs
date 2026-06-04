using ReportPortal.BL.Models;

namespace ReportPortal.BL.Services.Interfaces
{
    public interface ITestReviewService
    {
        public Task<TestReviewDto> GetTestReviewAsync(int testId, CancellationToken cancellationToken = default);
        public Task<TestReviewDto> UpdateTestReviewAsync(TestReviewUpdateDto testReviewUpdateDto, CancellationToken cancellationToken = default);
    }
}
