namespace ReportPortal.DAL.Repositories.Interfaces
{
    public interface IReporitory<T> where T : class
    {
        public Task<IEnumerable<T>> GetAllByIdAsync(int id, CancellationToken cancellationToken = default);
        public Task<T> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        public Task<int> InsertAsync(T user);
        public Task RemoveAsync(T user);
    }
}
