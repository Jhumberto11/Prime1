using API_PRIMECRM.Domain.Interfaces.Inventory;
using API_PRIMECRM.Domain.Models.Inventario;
using API_PRIMECRM.Domain.Models.Inventario.Enums;
using Microsoft.EntityFrameworkCore;
using PrimeCRM_Api.Infraestructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace API_PRIMECRM.Infraestructure.Persistence.Repositories.Inventory
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly AppDbContext _context;
        public InventoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(InventoryMovement movement)
        {
            await _context.InventoryMovements.AddAsync(movement);
            
        }

        public async Task<bool> ExistsMovementForSourceAsync(int productId, InventorySourceType sourceType, int sourceId, InventoryMovementType movementType)
        {
            return await _context.InventoryMovements
               .AsNoTracking()
               .AnyAsync(x =>
                   x.ProductID == productId &&
                   x.SourceType == sourceType &&
                   x.SourceId == sourceId &&
                   x.Type == movementType);
        }

        public async Task<IEnumerable<InventoryMovement>> GetAllAsync()
        {
            return await _context.InventoryMovements.AsNoTracking()
                .Include(x => x.Product)
                .OrderByDescending(x => x.Date)
                .ThenByDescending(x => x.Id)
                .ToListAsync();
        }

        public async Task<InventoryMovement?> GetByIdAsync(int id)
        {
            return await _context.InventoryMovements
                .AsNoTracking()
                .Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<InventoryMovement>> GetByProductIdAsync(int productId)
        {
            return await _context.InventoryMovements
                .AsNoTracking()
                .Include(x => x.Product)
                .Where(x => x.ProductID == productId)
                .OrderByDescending(x => x.Date)
                .ThenByDescending(x => x.Id)
                .ToListAsync();

        }

        public async Task<IEnumerable<InventoryMovement>> GetBySourceAsync(InventorySourceType sourceType)
        {
            return await _context.InventoryMovements
                .AsNoTracking()
                .Include(x => x.Product)
                .Where(x => x.SourceType == sourceType)
                .OrderByDescending(x => x.Date)
                .ThenByDescending(x => x.Id)
                .ToListAsync();




        }


        // =========================
        // VALOR DEL INVENTARIO
        // =========================


        public async Task<decimal> GetCurrentInventoryValueAsync(int productId)
        {
            var summary = await GetInventorySummaryAsync(productId);

            return Math.Round(
               summary.InventoryValue,
               2,
               MidpointRounding.AwayFromZero);
        }

        public async Task<decimal?> GetCurrentAverageUnitCostAsync(int productId)
        {
            var summary =
                await GetInventorySummaryAsync(productId);

            if (summary.Stock <= 0)
            {
                return null;
            }

            return Math.Round(
                summary.InventoryValue / summary.Stock,
                2,
                MidpointRounding.AwayFromZero);
        }

       

        public async Task<int> GetCurrentStockAsync(int productId)
        {
            var summary = await GetInventorySummaryAsync(productId);
            return summary.Stock;
        }




        // =========================
        // RESUMEN INTERNO
        // =========================

        private async Task<InventorySummary>
            GetInventorySummaryAsync(int productId)
        {
            var summary =
                await _context.InventoryMovements
                    .AsNoTracking()
                    .Where(x => x.ProductID == productId)
                    .GroupBy(x => 1)
                    .Select(group => new InventorySummary
                    {
                        Stock = group.Sum(x =>

                            x.Type == InventoryMovementType.RestockIn ||
                            x.Type == InventoryMovementType.SaleReturn ||
                            x.Type == InventoryMovementType.AdjustmentIn ||
                            x.Type == InventoryMovementType.InitialStock

                                ? x.Quantity

                                : x.Type == InventoryMovementType.SaleOut ||
                                  x.Type == InventoryMovementType.AdjustmentOut

                                    ? -x.Quantity
                                    : 0
                        ),

                        InventoryValue = group.Sum(x =>

                            x.Type == InventoryMovementType.RestockIn ||
                            x.Type == InventoryMovementType.SaleReturn ||
                            x.Type == InventoryMovementType.AdjustmentIn ||
                            x.Type == InventoryMovementType.InitialStock

                                ? x.Quantity * x.UnitCost

                                : x.Type == InventoryMovementType.SaleOut ||
                                  x.Type == InventoryMovementType.AdjustmentOut

                                    ? -(x.Quantity * x.UnitCost)
                                    : 0m
                        )
                    })
                    .FirstOrDefaultAsync();

            return summary ?? new InventorySummary();
        }


        private sealed class InventorySummary
        {
            public int Stock { get; set; }

            public decimal InventoryValue { get; set; }
        }



    }
}
