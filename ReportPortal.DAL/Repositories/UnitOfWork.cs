using Microsoft.EntityFrameworkCore;
using ReportPortal.DAL.Repositories.Interfaces;

namespace ReportPortal.DAL.Repositories
{
    public class UnitOfWork : AbstractApplicationRepository, IUnitOfWork
    {
        public UnitOfWork(ApplicationContext dbContext) : base(dbContext)
        {
        }

        public async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken = default)
        {
            // Repositories share this scoped DbContext, so their SaveChanges calls
            // enlist in the transaction opened here until it is committed.
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await operation(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
