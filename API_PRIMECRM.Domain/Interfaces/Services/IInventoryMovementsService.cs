using API_PRIMECRM.Domain.DTOs.Inventory;
using API_PRIMECRM.Domain.Models.Inventario;
using System;
using System.Collections.Generic;
using System.Text;

namespace API_PRIMECRM.Domain.Interfaces.Services
{
    public interface IInventoryMovementsService
    {
        Task<IEnumerable<ActualInventoryDto>> GetActualInventory();
    }
}
