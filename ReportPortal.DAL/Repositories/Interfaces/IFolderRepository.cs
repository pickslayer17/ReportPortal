using ReportPortal.DAL.Models.RunProjectManagement;
using System.Linq.Expressions;

namespace ReportPortal.DAL.Repositories.Interfaces
{
    public interface IFolderRepository : IRepository<Folder>
    {
        public Task<bool> ExistsAsync(Expression<Func<Folder, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>Flat load (Id, Name, ParentId, FolderLevel) of all folders in a run, for building the tree cache.</summary>
        Task<List<Folder>> GetByRunAsync(int runId, CancellationToken cancellationToken = default);

        Task InsertRangeAsync(IEnumerable<Folder> folders, CancellationToken cancellationToken = default);
    }
}
