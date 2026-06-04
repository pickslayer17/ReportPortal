
using ReportPortal.DAL.Models.RunProjectManagement;

namespace ReportPortal.DAL.Repositories.Interfaces
{
    public interface ITestReviewRepository: IRepository<TestReview>
    {
        Task InsertRangeAsync(IEnumerable<TestReview> testReviews, CancellationToken cancellationToken = default);
    }
}
