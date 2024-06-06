using Microsoft.EntityFrameworkCore;
using ReportPortal.DAL.Models.RunProjectManagement;
using ReportPortal.DAL.Repositories.Interfaces;
using System.Linq.Expressions;

namespace ReportPortal.DAL.Repositories
{
    public class RunRepository : AbstractApplicationRepository, IRunRepository
    {
        public RunRepository(ApplicationContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<Run>> GetAllByAsync(Expression<Func<Run, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Runs.Where(predicate).ToListAsync();
        }

        public async Task<Run> GetByAsync(Expression<Func<Run, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Runs.FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public async Task<int> InsertAsync(Run item)
        {
            _dbContext.Runs.Add(item);
            await _dbContext.SaveChangesAsync();

            return item.Id;
        }

        public async Task RemoveAsync(Run item)
        {
            _dbContext.Runs.Remove(item);
            await _dbContext.SaveChangesAsync();
        }
    }

}
