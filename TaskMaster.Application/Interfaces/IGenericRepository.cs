// TaskMaster.Application/Interfaces/IGenericRepository.cs

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaskMaster.Application.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
    }
}