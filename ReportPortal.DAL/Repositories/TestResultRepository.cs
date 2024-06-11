using Microsoft.EntityFrameworkCore;
using ReportPortal.DAL.Exceptions;
using ReportPortal.DAL.Models.RunProjectManagement;
using ReportPortal.DAL.Repositories.Interfaces;
using System.Linq.Expressions;

namespace ReportPortal.DAL.Repositories
{
    public class TestResultRepository : AbstractApplicationRepository, ITestResultRepository
    {
        public TestResultRepository(ApplicationContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<TestResult>> GetAllByAsync(Expression<Func<TestResult, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbContext.TestResults.Where(predicate).ToListAsync(cancellationToken);
        }

        public async Task<TestResult> GetByAsync(Expression<Func<TestResult, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var testResult =  await _dbContext.TestResults.FirstOrDefaultAsync(predicate, cancellationToken);
            if (testResult == null) throw new TestResultNotFoundException($"There is no test result with such predicate {predicate}");

            return testResult;
        }

        public async Task<int> InsertAsync(TestResult testResult)
        {
            _dbContext.TestResults.Add(testResult);
            await _dbContext.SaveChangesAsync();

            return testResult.Id;
        }

        public async Task RemoveByIdAsync(int testResultId)
        {
            var testResult = await GetByAsync(tr => tr.Id == testResultId);
            _dbContext.TestResults.Remove(testResult);
            await _dbContext.SaveChangesAsync();
        }

        public Task UpdateItem(TestResult item)
        {
            throw new NotImplementedException();
        }
    }
}
