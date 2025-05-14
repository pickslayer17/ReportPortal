using Microsoft.EntityFrameworkCore;
using ReportPortal.DAL.Exceptions;
using ReportPortal.DAL.Models.RunProjectManagement;
using ReportPortal.DAL.Repositories.Interfaces;
using System.Linq.Expressions;

namespace ReportPortal.DAL.Repositories
{
    public class ProjectRepository : AbstractApplicationRepository, IProjectRepository
    {
        public ProjectRepository(ApplicationContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<Project>> GetAllByAsync(Expression<Func<Project, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Projects.Where(predicate).ToListAsync(cancellationToken);
        }

        public async Task<Project> GetByAsync(Expression<Func<Project, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var project = await _dbContext.Projects.FirstOrDefaultAsync(predicate, cancellationToken);

            return project;
        }

        public async Task<int> InsertAsync(Project project, CancellationToken cancellationToken = default)
        {
            _dbContext.Projects.Add(project);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return project.Id;
        }

        public async Task RemoveByIdAsync(int projectId, CancellationToken cancellationToken = default)
        {
            var project = await GetByAsync(pr => pr.Id == projectId, cancellationToken);
            _dbContext.Projects.Remove(project);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public Task<Project> UpdateItemAsync(Project item, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
