using System;
using API_PRIMECRM.Domain.Models.Ventas;
namespace API_PRIMECRM.Domain.Interfaces.Sales;

public interface ISaleRepository
{
    Task<IEnumerable<Sale>> GetAllAsync();
    Task<Sale?> GetByIdAsync(int id);
    Task<Sale?> GetByIdForUpdateAsync(int id);
    Task<IEnumerable<Sale>>GetByProductIdAsync(int productId);
    Task AddAsync(Sale sale);
    Task<bool> ExistsAsync(int id);
    Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime from, DateTime to);

    Task<IEnumerable<Sale>>GetByStatusAsync(SaleStatus status);

    Task<IEnumerable<Sale>>GetDeliveredPendingSettlementByCourierAsync(int courierCompanyId);

    //Se puede agregar : GetByTracking
    //    GetByFundsLocalition
    //    GetByCourier








}
