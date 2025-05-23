﻿using Microsoft.EntityFrameworkCore;
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

        public Task<Run> UpdateItemAsync(Run item, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

}
