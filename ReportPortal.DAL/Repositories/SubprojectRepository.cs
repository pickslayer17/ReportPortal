using Microsoft.EntityFrameworkCore;
using ReportPortal.DAL.Models.RunProjectManagement;
using ReportPortal.DAL.Models.UserManagement;
using ReportPortal.DAL.Repositories.Interfaces;

namespace ReportPortal.DAL.Repositories
{
    public class SubprojectRepository : AbstractApplicationRepository, ISubprojectRepository
    {
        public SubprojectRepository(ApplicationContext dbContext) : base(dbContext)
        {
        }

        public async Task<int> InsertAsync(Subproject subproject, CancellationToken cancellationToken = default)
        {
            _dbContext.Subprojects.Add(subproject);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return subproject.Id;
        }

        public async Task<Subproject> GetByIdAsync(int subprojectId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Subprojects.FirstOrDefaultAsync(s => s.Id == subprojectId, cancellationToken);
        }

        public async Task<IEnumerable<Subproject>> GetForProjectAsync(int projectId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Subprojects.Where(s => s.ProjectId == projectId).ToListAsync(cancellationToken);
        }

        public async Task AddMemberAsync(int userId, int subprojectId, CancellationToken cancellationToken = default)
        {
            var exists = await _dbContext.UserSubprojects
                .AnyAsync(us => us.UserId == userId && us.SubprojectId == subprojectId, cancellationToken);
            if (exists) return;

            _dbContext.UserSubprojects.Add(new UserSubproject { UserId = userId, SubprojectId = subprojectId });
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveMemberAsync(int userId, int subprojectId, CancellationToken cancellationToken = default)
        {
            var membership = await _dbContext.UserSubprojects
                .FirstOrDefaultAsync(us => us.UserId == userId && us.SubprojectId == subprojectId, cancellationToken);
            if (membership == null) return;

            _dbContext.UserSubprojects.Remove(membership);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<User>> GetMembersAsync(int subprojectId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users
                .Where(u => u.UserSubprojects.Any(us => us.SubprojectId == subprojectId))
                .ToListAsync(cancellationToken);
        }
    }
}
