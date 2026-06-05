using Microsoft.EntityFrameworkCore;
using ReportPortal.DAL;
using ReportPortal.DAL.Exceptions;
using ReportPortal.DAL.Models.UserManagement;
using ReportPortal.DAL.Repositories.Interfaces;
using ReportPortal.Interfaces;
using System.Linq.Expressions;

namespace ReportPortal.Services
{
    public class UserRepository : AbstractApplicationRepository, IUserRepository
    {
        public UserRepository(ApplicationContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<User>> GetAllByAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users.Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersSharingProjectsAsync(int userId, CancellationToken cancellationToken = default)
        {
            var myProjectIds = _dbContext.UserProjects
                .Where(up => up.UserId == userId)
                .Select(up => up.ProjectId);

            // The user themselves is always included so the reviewer dropdown is never empty.
            return await _dbContext.Users
                .Where(u => u.Id == userId || u.UserProjects.Any(up => myProjectIds.Contains(up.ProjectId)))
                .ToListAsync(cancellationToken);
        }

        public async Task AddUserToProjectAsync(int userId, int projectId, CancellationToken cancellationToken = default)
        {
            var alreadyMember = await _dbContext.UserProjects
                .AnyAsync(up => up.UserId == userId && up.ProjectId == projectId, cancellationToken);
            if (alreadyMember) return;

            _dbContext.UserProjects.Add(new UserProject { UserId = userId, ProjectId = projectId });
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveUserFromProjectAsync(int userId, int projectId, CancellationToken cancellationToken = default)
        {
            var membership = await _dbContext.UserProjects
                .FirstOrDefaultAsync(up => up.UserId == userId && up.ProjectId == projectId, cancellationToken);
            if (membership == null) return;

            _dbContext.UserProjects.Remove(membership);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<User>> GetProjectMembersAsync(int projectId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users
                .Where(u => u.UserProjects.Any(up => up.ProjectId == projectId))
                .ToListAsync(cancellationToken);
        }

        public async Task<int> InsertAsync(User user, CancellationToken cancellationToken = default)
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return user.Id;
        }

        public async Task RemoveByIdAsync(int uesrId, CancellationToken cancellationToken = default)
        {
            var user = await GetByAsync(u => u.Id == uesrId, cancellationToken);
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<User> GetByAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(predicate, cancellationToken);
            if(user == null) throw new UserNotFoundException($"User with predicate {predicate} cannot be found.");

            return user;
        }

        public async Task<User> UpdateItemAsync(User item, CancellationToken cancellationToken = default)
        {
            _dbContext.Users.Update(item);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return item;
        }
    }
}
