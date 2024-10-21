using ReportPortal.DAL.Models.RunProjectManagement;
using ReportPortal.DAL.Repositories.Interfaces;
using System.Data.Entity;
using System.Linq.Expressions;

namespace ReportPortal.DAL.Repositories
{
    public class TestReviewRepository : AbstractApplicationRepository, ITestReviewRepository
    {
        private readonly ITestRepository _testRepository;

        public TestReviewRepository(ApplicationContext dbContext, ITestRepository testRepository) : base(dbContext)
        {
            _testRepository = testRepository;
        }

        public Task<IEnumerable<TestReview>> GetAllByAsync(Expression<Func<TestReview, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<TestReview> GetByAsync(Expression<Func<TestReview, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<int> InsertAsync(TestReview testReview)
        {
            var test = await _testRepository.GetByAsync(t => t.Id == testReview.Id);
            testReview.Test = test;
            _dbContext.TestReviews.Add(testReview);
            await _dbContext.SaveChangesAsync();

            return testReview.Id;
        }

        public Task RemoveByIdAsync(int itemId)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateItem(TestReview testReview)
        {
            var existingTestReview = await _dbContext.TestReviews.FirstOrDefaultAsync(tr => tr.Id == testReview.Id);
            existingTestReview = testReview;
            await _dbContext.SaveChangesAsync();
        }
    }
}
