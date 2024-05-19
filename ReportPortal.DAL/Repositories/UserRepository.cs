using Microsoft.EntityFrameworkCore;
using ReportPortal.DAL;
using ReportPortal.DAL.Models;
using ReportPortal.Interfaces;

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

        public async Task<User> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var userModel = _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);

            return await userModel;
        }

        public async Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var userModel = _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

            return await userModel;
        }

        public async Task InsertAsync(User user)
        {
            _dbContext.Users.Add(user);
            _dbContext.SaveChangesAsync();
        }

        public async Task RemoveAsync(User user)
        {
            _dbContext.Users.Remove(user);
            _dbContext.SaveChangesAsync();
        }
    }
}
