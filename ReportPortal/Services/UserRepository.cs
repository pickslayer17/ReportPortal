using ReportPortal.DAL;
using ReportPortal.Interfaces;

namespace ReportPortal.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationContext _dbContext;

        public UserRepository(ApplicationContext dbContext) => _dbContext = dbContext;

        public Task<IEnumerable<User>> GetAllByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetByIdAsync(Guid accountId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void Insert(User user)
        {
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
        }

        public void Remove(User user)
        {
            throw new NotImplementedException();
        }
    }
}
