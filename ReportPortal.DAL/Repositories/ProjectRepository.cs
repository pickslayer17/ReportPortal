using Microsoft.EntityFrameworkCore;
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
            return await _dbContext.Projects.Where(predicate).ToListAsync();
        }

        public Task<Project> GetByAsync(Expression<Func<Project, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<int> InsertAsync(Project project)
        {
            _dbContext.Projects.Add(project);
            await _dbContext.SaveChangesAsync();

            return project.Id;
        }

        public Task RemoveAsync(Project user)
        {
            throw new NotImplementedException();
        }
    }
}
