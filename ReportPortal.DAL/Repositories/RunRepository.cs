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
            return await _dbContext.Runs.Where(predicate).ToListAsync(cancellationToken);
        }

        public async Task<Run> GetByAsync(Expression<Func<Run, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Runs.FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public async Task<int> InsertAsync(Run item, CancellationToken cancellationToken = default)
        {
            _dbContext.Runs.Add(item);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return item.Id;
        }

        public async Task RemoveByIdAsync(int runId, CancellationToken cancellationToken = default)
        {
            var run = await GetByAsync(r => r.Id == runId, cancellationToken);
            _dbContext.Runs.Remove(run);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveRunCascadeAsync(int runId, CancellationToken cancellationToken = default)
        {
            // Delete children before parents to satisfy the FK graph:
            // TestResults/TestReviews -> Tests -> Folders -> Run.
            await _dbContext.TestResults.Where(tr => tr.Test.RunId == runId).ExecuteDeleteAsync(cancellationToken);
            await _dbContext.TestReviews.Where(rv => rv.Test.RunId == runId).ExecuteDeleteAsync(cancellationToken);
            await _dbContext.Tests.Where(t => t.RunId == runId).ExecuteDeleteAsync(cancellationToken);

            // Folders self-reference with DeleteBehavior.Restrict, so a run's folders
            // must be removed from the deepest level up to the root.
            var maxLevel = await _dbContext.Folders
                .Where(f => f.RunId == runId)
                .Select(f => (int?)f.FolderLevel)
                .MaxAsync(cancellationToken) ?? -1;

            for (var level = maxLevel; level >= 0; level--)
            {
                var currentLevel = level;
                await _dbContext.Folders
                    .Where(f => f.RunId == runId && f.FolderLevel == currentLevel)
                    .ExecuteDeleteAsync(cancellationToken);
            }

            await _dbContext.Runs.Where(r => r.Id == runId).ExecuteDeleteAsync(cancellationToken);
        }

        public Task<Run> UpdateItemAsync(Run item, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

}
