using ReportPortal.DAL.Models.RunProjectManagement;

namespace ReportPortal.DAL.Repositories.Interfaces
{
    public interface IRunRepository : IRepository<Run>
    {
        /// <summary>
        /// Deletes a run together with all of its folders, tests, results and reviews.
        /// Caller is responsible for wrapping this in a transaction.
        /// </summary>
        Task RemoveRunCascadeAsync(int runId, CancellationToken cancellationToken = default);
    }
}
