using API_PRIMECRM.Domain.Models.Inventario;
using API_PRIMECRM.Domain.Models.Inventario.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace API_PRIMECRM.Domain.Interfaces.Inventory
{
    public interface IInventoryRepository
    {
        Task<IEnumerable<InventoryMovement>> GetAllAsync();

        Task<InventoryMovement?> GetByIdAsync(int id);

        Task<IEnumerable<InventoryMovement>>GetByProductIdAsync(int productId);

        Task<IEnumerable<InventoryMovement>>GetBySourceAsync(InventorySourceType sourceType); //  int sourceid

        Task<int> GetCurrentStockAsync(int productId);

        Task<decimal> GetCurrentInventoryValueAsync(int productId);

        Task<decimal?> GetCurrentAverageUnitCostAsync(int productId);

        Task<bool> ExistsMovementForSourceAsync(int productId, InventorySourceType sourceType, int sourceId, InventoryMovementType movementType);

        Task AddAsync(InventoryMovement movement);
    }
}
