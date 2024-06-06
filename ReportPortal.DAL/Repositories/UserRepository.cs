using Microsoft.EntityFrameworkCore;
using ReportPortal.DAL;
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

        public async Task<int> InsertAsync(User user)
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return user.Id.Value;
        }

        public async Task RemoveAsync(User user)
        {
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<User> GetByAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(predicate);
        }
    }
}
