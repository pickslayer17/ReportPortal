namespace ReportPortal.DAL.Repositories.Interfaces
{
    /// <summary>
    /// Runs several repository operations as a single atomic unit so a partial
    /// failure cannot leave orphaned rows (e.g. a Run without its root folder).
    /// </summary>
    public interface IUnitOfWork
    {
        Task ExecuteInTransactionAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken = default);
    }
}
