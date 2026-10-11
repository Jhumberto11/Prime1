using API_PRIMECRM.Domain.DTOs.Sales;
using API_PRIMECRM.Domain.Models.Ventas;
using System;
using System.Collections.Generic;
using System.Text;

namespace API_PRIMECRM.Domain.Interfaces.Services
{
    public interface ISaleService
    {
        Task<IEnumerable<Sale>> GetAllAsync();

        Task<Sale> GetByIdAsync(int id);

        Task<IEnumerable<Sale>>GetByProductIdAsync(int productId);

        Task<IEnumerable<Sale>>GetByDateRangeAsync(DateTime from,DateTime to);

        Task<IEnumerable<Sale>>GetByStatusAsync(SaleStatus status);

        Task<Sale> CreateAsync(CreateSaleDto dto);

        Task UpdateTrackingAsync(int saleId, string trackingNumber);

        Task MarkAsDeliveredAsync(int saleId);

        Task MarkAsNotDeliveredAsync(int saleId);

        Task MarkAsLostAsync(int saleId);
    }
}
