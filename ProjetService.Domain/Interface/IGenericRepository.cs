﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjetService.Domain.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task DeleteByIdAsync(int id);
    }
}
