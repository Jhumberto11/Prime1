using API_PRIMECRM.Domain.DTOs;
using API_PRIMECRM.Domain.Models.Reabastecimimento;
using System;
using System.Collections.Generic;
using System.Text;

namespace API_PRIMECRM.Domain.Interfaces.Services
{
    public interface IRestockOrderService
    {
        Task<IEnumerable<RestockOrder>> GetAllAsync();

        Task<RestockOrder?> GetByIdAsync(int id);

        Task<IEnumerable<RestockOrder>> GetByProductIdAsync(int productId);

        Task<RestockOrder> CreateAsync(CreateRestockOrderDto dto);

        Task<RestockOrder?> RegisterActualFreightAsync(
            int id,
            decimal actualFreight);
    }
}
