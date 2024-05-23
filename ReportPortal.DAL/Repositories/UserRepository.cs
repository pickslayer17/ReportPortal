using Microsoft.EntityFrameworkCore;
using ReportPortal.DAL;
using ReportPortal.DAL.Models;
using ReportPortal.Interfaces;
using System.Linq.Expressions;

namespace ReportPortal.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationContext _dbContext;

        public UserRepository(ApplicationContext dbContext) => _dbContext = dbContext;

        public Task<IEnumerable<User>> GetAllByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
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
            var userModel = _dbContext.Users.FirstOrDefaultAsync(predicate);

            return await userModel;
        }
    }
}
