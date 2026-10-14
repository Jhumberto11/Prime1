using API_PRIMECRM.Domain.DTOs.Inventory;
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

        public async Task<IEnumerable<InventoryMovement>> GetBySourceAsync(InventoryMovementType sourceType)
        {
            return await _context.InventoryMovements
                .AsNoTracking()
                .Include(x => x.Product)
                .Where(x => x.Type == sourceType)
                .OrderByDescending(x => x.Date)
                .ThenByDescending(x => x.Id)
                .ToListAsync();




        }


        // =========================
        // VALOR DEL INVENTARIO
        // =========================


       



        /// <summary>
        /// Inventario Total
        /// </summary>
        /// <returns></returns>
        public async Task<List<ActualInventoryDto>> GetInventorySummaryAsync()
        {
            return await _context.InventoryMovements
                .AsNoTracking()
                .GroupBy(m => new
                {
                    m.ProductID,
                    m.Product.Name
                })
                .Select(g => new ActualInventoryDto
                {
                    ProductId = g.Key.ProductID,
                    ProductName = g.Key.Name,

                    TotalEntries = g.Sum(m =>
                        m.Type == InventoryMovementType.AdjustmentIn || m.Type == InventoryMovementType.InitialStock || m.Type == InventoryMovementType.RestockIn || m.Type == InventoryMovementType.SaleReturn
                            ? m.Quantity : 0),

                    TotalExits = g.Sum(m =>
                        m.Type == InventoryMovementType.SaleOut || m.Type == InventoryMovementType.AdjustmentOut
                            ? m.Quantity : 0),

                    ActualStock = g.Sum(m =>
                                        m.Type == InventoryMovementType.RestockIn ||
                                        m.Type == InventoryMovementType.SaleReturn ||
                                        m.Type == InventoryMovementType.AdjustmentIn ||
                                        m.Type == InventoryMovementType.InitialStock
                                            ? m.Quantity
                                            : m.Type == InventoryMovementType.SaleOut ||
                                              m.Type == InventoryMovementType.AdjustmentOut
                                                ? -m.Quantity
                                                : 0),

                    LastMovementDate = g.Max(m => m.Date)
                })
                .ToListAsync();
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

        public async Task<decimal?> GetCurrentAverageUnitCostAsync(int productId)
        {
            var summary = await GetInventorySummaryAsync(productId);

            if (summary.Stock <= 0)
            {
                return null;
            }

            var today = DateOnly.FromDateTime(DateTime.Today);

            int[] months = { 3, 6, 9, 12 };

            foreach (var month in months)
            {
                var startDate = today.AddMonths(-month);

                var costs = await _context.InventoryMovements
                    .AsNoTracking()
                    .Where(x =>
                        x.ProductID == productId &&
                        x.Type == InventoryMovementType.RestockIn &&
                        x.Date >= startDate &&
                        x.Date <= today &&
                        x.Quantity > 0 &&
                        x.UnitCost > 0)
                    .GroupBy(x => x.ProductID)
                    .Select(g => new
                    {
                        TotalQuantity = g.Sum(x => x.Quantity),

                        TotalCost = g.Sum(x =>
                            x.Quantity * x.UnitCost)
                    })
                    .FirstOrDefaultAsync();

                if (costs != null && costs.TotalQuantity > 0)
                {
                    return Math.Round(
                        costs.TotalCost / costs.TotalQuantity,
                        2,
                        MidpointRounding.AwayFromZero);
                }
            }

            return null;
        }



        private sealed class InventorySummary
        {
            public int Stock { get; set; }

            public decimal InventoryValue { get; set; }
        }



    }
}
