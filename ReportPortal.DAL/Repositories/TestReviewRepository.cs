using ReportPortal.DAL.Models.RunProjectManagement;
using ReportPortal.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ReportPortal.DAL.Repositories
{
    public class TestReviewRepository : AbstractApplicationRepository, ITestReviewRepository
    {

        public TestReviewRepository(ApplicationContext dbContext) : base(dbContext)
        {
        }

        public Task<IEnumerable<TestReview>> GetAllByAsync(Expression<Func<TestReview, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<TestReview> GetByAsync(Expression<Func<TestReview, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var testReview = await _dbContext.TestReviews.FirstOrDefaultAsync(predicate, cancellationToken);

            return testReview;
        }

        public async Task<int> InsertAsync(TestReview testReview)
        {
            _dbContext.TestReviews.Add(testReview);
            await _dbContext.SaveChangesAsync();

            return testReview.Id;
        }

        public Task RemoveByIdAsync(int itemId)
        {
            throw new NotImplementedException();
        }

        public async Task<TestReview> UpdateItem(TestReview testReview)
        {
            var existingTestReview = await _dbContext.TestReviews.FirstOrDefaultAsync(tr => tr.Id == testReview.Id);
            //existingTestReview.TestReviewOutcome = testReview.TestReviewOutcome;
            //existingTestReview.Comments = testReview.Comments;
            //existingTestReview.ReviewerId = testReview.ReviewerId;
            _dbContext.TestReviews.Entry(existingTestReview).CurrentValues.SetValues(testReview);
            await _dbContext.SaveChangesAsync();

            return existingTestReview;
        }
    }
}
