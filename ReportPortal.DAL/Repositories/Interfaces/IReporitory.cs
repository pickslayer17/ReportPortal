using System;
using System.Linq.Expressions;

namespace ReportPortal.DAL.Repositories.Interfaces
{
    public interface IReporitory<T> where T : class
    {
        public Task<IEnumerable<T>> GetAllByIdAsync(int id, CancellationToken cancellationToken = default);
        public Task<T> GetByAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        public Task<int> InsertAsync(T user);
        public Task RemoveAsync(T user);
    }
}
