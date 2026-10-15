using API_PRIMECRM.Domain.DTOs.Inventory;
using API_PRIMECRM.Domain.Interfaces.Inventory;
using API_PRIMECRM.Domain.Interfaces.Services;
using API_PRIMECRM.Domain.Models.Inventario;
using API_PRIMECRM.Domain.Models.Inventario.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace API_PRIMECRM.Application.Services.InventoryMovements
{
    public class InventoryMovementService : IInventoryMovementsService
    {
        private readonly IInventoryRepository _repository;

        public InventoryMovementService(
            IInventoryRepository repository)
        {
            _repository = repository;
        }

        public  async Task<IEnumerable<ActualInventoryDto>> GetActualInventory()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            // 1. Obtener todos los movimientos
            var movements = await _repository.GetAllAsync();

            // 2. Agrupar los movimientos por producto
            var inventory = movements
                .GroupBy(m => m.ProductID)
                .Select(group =>
                {
                    // Total de entradas
                    int entries = group
                        .Where(m =>
                            (m.Type == InventoryMovementType.RestockIn && m.OrderStatus == OrderStatus.Delivered)
                            || m.Type == InventoryMovementType.AdjustmentIn
                            || m.Type == InventoryMovementType.SaleReturn
                            || m.Type == InventoryMovementType.InitialStock)
                        .Sum(m => m.Quantity);
                    

                    // Total de salidas
                    int exits = group
                        .Where(m => m.Type == InventoryMovementType.SaleOut 
                        || m.Type == InventoryMovementType.AdjustmentOut)
                        .Sum(m => m.Quantity);

                    
                    // 3. COMPRAS ENTREGADAS CON COSTO VÁLIDO
                    var restocks = group
                        .Where(m =>
                            m.Type == InventoryMovementType.RestockIn
                            && m.OrderStatus == OrderStatus.Delivered
                            && m.Quantity > 0
                            && m.Date <= today)
                        .ToList();

                    decimal? averageCost = null;
                    decimal? totalCost = null;

                    if (restocks.Any())
                    {
                        // Comenzamos buscando 3 meses atrás
                        int monthsBack = 3;

                        DateOnly fromDate = today.AddMonths(-monthsBack);

                        // Fecha del último Restock recibido
                        DateOnly lastRestockDate = restocks.Max(m => m.Date);

                        // Si no hay compras en ese período,
                        // ampliar el rango un mes cada vez.
                        while (fromDate > lastRestockDate)
                        {
                            monthsBack++;
                            fromDate = today.AddMonths(-monthsBack);
                        }

                        // Compras dentro del período encontrado
                        var selectedRestocks = restocks
                            .Where(m => m.Date >= fromDate)
                            .ToList();

                        // Promedio ponderado por cantidad
                        totalCost = selectedRestocks.Sum(
                            m => m.UnitCost * m.Quantity);

                        int totalQuantity = selectedRestocks.Sum(
                            m => m.Quantity);

                        averageCost = totalCost / totalQuantity;
                    }

                    return new ActualInventoryDto
                    {
                        ProductId = group.Key,
                        ProductName = group.First().Product.Name,

                        TotalEntries = entries,
                        TotalExits = exits,

                        ActualStock = entries - exits,

                        AverageRestockCost = averageCost,

                        ActualCostStock = totalCost,



                        LastMovementDate = group.Max(m => m.Date)
                    };
                })
                .OrderBy(x => x.ProductName)
                .ToList();



                    

            return inventory;
        }

        
    }
}
