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
        // The chain now runs through the subproject: Run -> Subproject -> Project.
        public async Task<int?> ResolveProjectIdAsync(ScopeResource resource, int id, CancellationToken cancellationToken = default)
        {
            switch (resource)
            {
                case ScopeResource.Project:
                    return await _dbContext.Projects
                        .Where(p => p.Id == id).Select(p => (int?)p.Id)
                        .FirstOrDefaultAsync(cancellationToken);

                case ScopeResource.Subproject:
                    return await _dbContext.Subprojects
                        .Where(s => s.Id == id).Select(s => (int?)s.ProjectId)
                        .FirstOrDefaultAsync(cancellationToken);

                case ScopeResource.Run:
                    return await (from r in _dbContext.Runs
                                  join sp in _dbContext.Subprojects on r.SubprojectId equals sp.Id
                                  where r.Id == id
                                  select (int?)sp.ProjectId)
                                 .FirstOrDefaultAsync(cancellationToken);

                case ScopeResource.Folder:
                    return await (from f in _dbContext.Folders
                                  join r in _dbContext.Runs on f.RunId equals r.Id
                                  join sp in _dbContext.Subprojects on r.SubprojectId equals sp.Id
                                  where f.Id == id
                                  select (int?)sp.ProjectId)
                                 .FirstOrDefaultAsync(cancellationToken);

                case ScopeResource.Test:
                    return await (from t in _dbContext.Tests
                                  join r in _dbContext.Runs on t.RunId equals r.Id
                                  join sp in _dbContext.Subprojects on r.SubprojectId equals sp.Id
                                  where t.Id == id
                                  select (int?)sp.ProjectId)
                                 .FirstOrDefaultAsync(cancellationToken);

                case ScopeResource.TestResult:
                    return await (from tr in _dbContext.TestResults
                                  join t in _dbContext.Tests on tr.TestId equals t.Id
                                  join r in _dbContext.Runs on t.RunId equals r.Id
                                  join sp in _dbContext.Subprojects on r.SubprojectId equals sp.Id
                                  where tr.Id == id
                                  select (int?)sp.ProjectId)
                                 .FirstOrDefaultAsync(cancellationToken);

                case ScopeResource.TestReview:
                    return await (from rev in _dbContext.TestReviews
                                  join t in _dbContext.Tests on rev.TestId equals t.Id
                                  join r in _dbContext.Runs on t.RunId equals r.Id
                                  join sp in _dbContext.Subprojects on r.SubprojectId equals sp.Id
                                  where rev.Id == id
                                  select (int?)sp.ProjectId)
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

        public async Task<int?> ResolveSubprojectIdForReviewAsync(int reviewId, CancellationToken cancellationToken = default)
        {
            return await (from rev in _dbContext.TestReviews
                          join t in _dbContext.Tests on rev.TestId equals t.Id
                          join r in _dbContext.Runs on t.RunId equals r.Id
                          where rev.Id == reviewId
                          select (int?)r.SubprojectId)
                         .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> IsSubprojectMemberAsync(int userId, int subprojectId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.UserSubprojects
                .AnyAsync(us => us.UserId == userId && us.SubprojectId == subprojectId, cancellationToken);
        }
    }
}
