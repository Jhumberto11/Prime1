using API_PRIMECRM.Domain.Models.Inventario;
using System;
using System.Collections.Generic;
using System.Text;

namespace API_PRIMECRM.Domain.Interfaces.Services
{
    public interface IInventoryMovementsService
    {
        Task<IEnumerable<InventoryMovement>> GetActualInventory();
    }
}
