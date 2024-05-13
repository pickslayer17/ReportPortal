using ReportPortal.DAL;

namespace ReportPortal.Interfaces
{
    public interface IUserRepository
    {
        public Task<IEnumerable<User>> GetAllByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);
        public Task<User> GetByIdAsync(Guid accountId, CancellationToken cancellationToken = default);
        public void Insert(User user);
        public void Remove(User user);
    }
}
