using ReportPortal.BL.Models;
using System.Linq.Expressions;

namespace ReportPortal.BL.Services.Interfaces
{
    public interface IServiceBase<T, C, FC> where T : class
    {
        public Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
        public Task<IEnumerable<T>> GetAllByAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        public Task<T> GetByAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        public Task<C> CreateAsync(FC projectForCreationDto, CancellationToken cancellationToken = default);
        public Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }

}
