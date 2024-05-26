﻿using System;
using System.Linq.Expressions;

namespace ReportPortal.DAL.Repositories.Interfaces
{
    public interface IReporitory<T> where T : class
    {
        public Task<IEnumerable<T>> GetAllByAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        public Task<T> GetByAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        public Task<int> InsertAsync(T item);
        public Task RemoveAsync(T item);
    }
}
