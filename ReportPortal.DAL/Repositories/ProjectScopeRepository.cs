using Microsoft.EntityFrameworkCore;
using ReportPortal.DAL.Repositories.Interfaces;

namespace ReportPortal.DAL.Repositories
{
    public class ProjectScopeRepository : AbstractApplicationRepository, IProjectScopeRepository
    {
        public ProjectScopeRepository(ApplicationContext dbContext) : base(dbContext)
        {
        }

        // Each branch is a single indexed lookup that walks the resource up to its project.
        // Runs belong directly to a project: Run -> Project.
        public async Task<int?> ResolveProjectIdAsync(ScopeResource resource, int id, CancellationToken cancellationToken = default)
        {
            switch (resource)
            {
                case ScopeResource.Project:
                    return await _dbContext.Projects
                        .Where(p => p.Id == id).Select(p => (int?)p.Id)
                        .FirstOrDefaultAsync(cancellationToken);

                case ScopeResource.Run:
                    return await _dbContext.Runs
                        .Where(r => r.Id == id).Select(r => (int?)r.ProjectId)
                        .FirstOrDefaultAsync(cancellationToken);

                case ScopeResource.Folder:
                    return await (from f in _dbContext.Folders
                                  join r in _dbContext.Runs on f.RunId equals r.Id
                                  where f.Id == id
                                  select (int?)r.ProjectId)
                                 .FirstOrDefaultAsync(cancellationToken);

                case ScopeResource.Test:
                    return await (from t in _dbContext.Tests
                                  join r in _dbContext.Runs on t.RunId equals r.Id
                                  where t.Id == id
                                  select (int?)r.ProjectId)
                                 .FirstOrDefaultAsync(cancellationToken);

                case ScopeResource.TestResult:
                    return await (from tr in _dbContext.TestResults
                                  join t in _dbContext.Tests on tr.TestId equals t.Id
                                  join r in _dbContext.Runs on t.RunId equals r.Id
                                  where tr.Id == id
                                  select (int?)r.ProjectId)
                                 .FirstOrDefaultAsync(cancellationToken);

                case ScopeResource.TestReview:
                    return await (from rev in _dbContext.TestReviews
                                  join t in _dbContext.Tests on rev.TestId equals t.Id
                                  join r in _dbContext.Runs on t.RunId equals r.Id
                                  where rev.Id == id
                                  select (int?)r.ProjectId)
                                 .FirstOrDefaultAsync(cancellationToken);

                default:
                    return null;
            }
        }

        public async Task<bool> IsMemberAsync(int userId, int projectId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.UserProjects
                .AnyAsync(up => up.UserId == userId && up.ProjectId == projectId, cancellationToken);
        }
    }
}
