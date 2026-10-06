using API_PRIMECRM.Domain.Models.Reabastecimimento;
using System;
using System.Collections.Generic;
using System.Text;

namespace API_PRIMECRM.Domain.Interfaces.Restock
{
    public interface IRestockRepository
    {
        Task<IEnumerable<RestockOrder>> GetAllAsync();

        Task<RestockOrder?> GetByIdAsync(int id);
        Task<RestockOrder> GetByIdForUpdateAsync(int id);

        Task<IEnumerable<RestockOrder>> GetByProductIdAsync(int productId);

        Task AddAsync(RestockOrder restockOrder);

        Task<RestockOrder?> UpdateAsync(RestockOrder restockOrder);

        Task<bool> ExistsAsync(int id);

    }
}
