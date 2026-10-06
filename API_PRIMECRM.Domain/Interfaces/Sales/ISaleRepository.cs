using System;
using API_PRIMECRM.Domain.Models.Ventas;
namespace API_PRIMECRM.Domain.Interfaces.Sales;

public interface ISaleRepository
{
    Task<IEnumerable<Sale>> GetAllAsync();
    Task<Sale?> GetByIdAsync(int id);
    Task<Sale> GetByIdForUpdateAsync(int id);
    Task<IEnumerable<Sale>> GetByCustomerIdAsync(int customerId);
    Task AddAsync(Sale sale);
    Task<Sale?> UpdateAsync(Sale sale);
    Task<bool> ExistsAsync(int id);
    Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    





}
