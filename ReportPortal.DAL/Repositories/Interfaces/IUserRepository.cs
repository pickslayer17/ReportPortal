using ReportPortal.DAL.Models;
using ReportPortal.DAL.Repositories.Interfaces;

namespace ReportPortal.Interfaces
{
    public interface IUserRepository : IReporitory<User>
    {
        public Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    }
}
