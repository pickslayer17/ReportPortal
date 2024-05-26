using ReportPortal.DAL.Models;
using ReportPortal.DAL.Models.TestResult;
using ReportPortal.DAL.Repositories.Interfaces;
using System.Linq.Expressions;

namespace ReportPortal.DAL.Repositories
{
    public class TestRepository : AbstractApplicationRepository, ITestRepository
    {
        public TestRepository(ApplicationContext dbContext) : base(dbContext)
        {
        }

        public Task<IEnumerable<TestRunItem>> GetAllByAsync(Expression<Func<TestRunItem, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<TestRunItem> GetByAsync(Expression<Func<TestRunItem, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<int> InsertAsync(TestRunItem testRunItem)
        {
            _dbContext.Tests.Add(testRunItem);
            await _dbContext.SaveChangesAsync();

            return testRunItem.Id;
        }

        public Task RemoveAsync(TestRunItem user)
        {
            throw new NotImplementedException();
        }
    }
}
