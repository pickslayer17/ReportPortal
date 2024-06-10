using Microsoft.EntityFrameworkCore;
using ReportPortal.DAL.Models.RunProjectManagement;
using ReportPortal.DAL.Repositories.Interfaces;
using System.Linq.Expressions;

namespace ReportPortal.DAL.Repositories
{
    public class TestRepository : AbstractApplicationRepository, ITestRepository
    {
        public TestRepository(ApplicationContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<TestRunItem>> GetAllByAsync(Expression<Func<TestRunItem, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Tests.Where(predicate).ToListAsync(cancellationToken);
        }

        public async Task<TestRunItem> GetByAsync(Expression<Func<TestRunItem, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Tests.FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public async Task<int> InsertAsync(TestRunItem testRunItem)
        {
            _dbContext.Tests.Add(testRunItem);
            await _dbContext.SaveChangesAsync();

            return testRunItem.Id;
        }

        public async Task RemoveByIdAsync(int testId)
        {
            var test = await GetByAsync(t => t.Id == testId);
            _dbContext.Tests.Remove(test);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateItem(TestRunItem item)
        {
            var oldItem = await GetByAsync(t => t.Id == item.Id);
            _dbContext.Tests.Entry(oldItem).CurrentValues.SetValues(item);
            await _dbContext.SaveChangesAsync();
        }
    }
}
