using System.Linq.Expressions;

namespace ReportPortal.BL.Services.Interfaces
{
    public interface IServiceBase<T, C> where T : class
    {
        public Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
        public Task<IEnumerable<T>> GetAllByAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        public Task<T> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        public Task<T> GetByAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        public Task<C> CreateAsync(T projectForCreationDto, CancellationToken cancellationToken = default);
        public Task DeleteByIdAsync(int id, CancellationToken cancellationToken = default);
    }

}
