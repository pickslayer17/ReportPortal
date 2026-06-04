using ReportPortal.DAL.Models.RunProjectManagement;
using System.Linq.Expressions;

namespace ReportPortal.DAL.Repositories.Interfaces
{
    public interface ITestRepository : IRepository<Test>
    {
        Task<bool> ExistsAsync(Expression<Func<Test, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>Lightweight load (Id, Name, FolderId) of all tests in a run, for dedup during ingest.</summary>
        Task<List<Test>> GetByRunAsync(int runId, CancellationToken cancellationToken = default);

        /// <summary>Per-folder test counters for a run (latest outcome + review outcome), computed on the backend.</summary>
        Task<List<FolderTestStats>> GetFolderStatsByRunAsync(int runId, CancellationToken cancellationToken = default);

        Task InsertRangeAsync(IEnumerable<Test> tests, CancellationToken cancellationToken = default);
    }
}
