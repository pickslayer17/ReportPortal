using ReportPortal.DAL.Models;
using ReportPortal.DAL.Repositories.Interfaces;
using System.Linq.Expressions;

namespace ReportPortal.DAL.Repositories
{
    public class TestResultRepository : AbstractApplicationRepository, ITestResultRepository
    {
        public TestResultRepository(ApplicationContext dbContext) : base(dbContext)
        {
        }

        public Task<IEnumerable<TestResult>> GetAllByAsync(Expression<Func<TestResult, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<TestResult> GetByAsync(Expression<Func<TestResult, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<int> InsertAsync(TestResult testResult)
        {
            _dbContext.TestResults.Add(testResult);
            await _dbContext.SaveChangesAsync();

            return testResult.Id;
        }

        public Task RemoveAsync(TestResult user)
        {
            throw new NotImplementedException();
        }
    }
}
