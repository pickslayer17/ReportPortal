using ReportPortal.DAL.Models.RunProjectManagement;
using System.Linq.Expressions;

namespace ReportPortal.DAL.Repositories.Interfaces
{
    public interface IFolderRepository : IRepository<Folder>
    {
        public Task<bool> ExistsAsync(Expression<Func<Folder, bool>> predicate, CancellationToken cancellationToken = default);
    }
}
