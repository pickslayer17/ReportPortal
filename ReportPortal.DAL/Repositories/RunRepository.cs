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

        public Task<IEnumerable<Run>> GetAllByAsync(Expression<Func<Run, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Run> GetByAsync(Expression<Func<Run, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<int> InsertAsync(Run item)
        {
            _dbContext.Runs.Add(item);
            await _dbContext.SaveChangesAsync();

            return item.Id;
        }

        public Task RemoveAsync(Run item)
        {
            throw new NotImplementedException();
        }
    }

}
