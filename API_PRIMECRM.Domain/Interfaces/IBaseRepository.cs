using API_PRIMECRM.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace API_PRIMECRM.Domain.Interfaces
{
    public interface IBaseRepository<TEntity> where TEntity  : Base
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity?> GetByIdAsync(int id);
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);      


    }
}
