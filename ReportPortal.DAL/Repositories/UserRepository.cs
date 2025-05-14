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

        public async Task<int> InsertAsync(User user, CancellationToken cancellationToken = default)
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return user.Id.Value;
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

        public Task<User> UpdateItemAsync(User item, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
