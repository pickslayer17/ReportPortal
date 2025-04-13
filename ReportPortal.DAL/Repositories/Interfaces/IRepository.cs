using System.Linq.Expressions;

namespace ReportPortal.DAL.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        public Task<IEnumerable<T>> GetAllByAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        public Task<T> GetByAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        public Task<int> InsertAsync(T item, CancellationToken cancellationToken = default);
        public Task RemoveByIdAsync(int itemId, CancellationToken cancellationToken = default);
        public Task<T> UpdateItemAsync(T item, CancellationToken cancellationToken = default);
    }
}
