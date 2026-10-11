using API_PRIMECRM.Domain.Interfaces.Sales;
using API_PRIMECRM.Domain.Models.Ventas;
using Microsoft.EntityFrameworkCore;
using PrimeCRM_Api.Infraestructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace API_PRIMECRM.Infraestructure.Persistence.Repositories.Sales
{
    public class SaleRepository : ISaleRepository
    {
        private readonly AppDbContext _context;
        public SaleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Sale sale)
        {
            await _context.AddAsync(sale);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Sales
                .AsNoTracking()
                .AnyAsync(x => x.Id == id);
        }

        // =========================
        // TODAS LAS VENTAS
        // =========================

        public async Task<IEnumerable<Sale>>
            GetAllAsync()
        {
            return await GetReadQuery()
                .OrderByDescending(x => x.SaleDate)
                .ThenByDescending(x => x.Id)
                .ToListAsync();
        }

        public Task<IEnumerable<Sale>> GetByCustomerIdAsync(int customerId)
        {
            throw new NotImplementedException();
        }

        // =========================
        // VENTAS POR FECHAS
        // =========================

        public async Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime from, DateTime to)
        {
            return await GetReadQuery()
                .Where(x => x.SaleDate >= from && x.SaleDate <= to)
                .OrderByDescending(x => x.SaleDate)
                .ThenByDescending(x => x.Id)
                .ToListAsync();
        }

        // =========================
        // VENTA POR ID - SOLO LECTURA
        // =========================

        public async Task<Sale?>
            GetByIdAsync(int id)
        {
            return await GetReadQuery()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        // =========================
        // VENTA PARA MODIFICAR
        // =========================

        public async Task<Sale?>GetByIdForUpdateAsync(int id)
        {
            var sale = await _context.Sales.FirstOrDefaultAsync(x => x.Id == id);
            return sale;
        }

        // =========================
        // VENTAS POR PRODUCTO
        // =========================


        public async Task<IEnumerable<Sale>> GetByProductIdAsync(int productId)
        {
            return await GetReadQuery()
                .Where(x => x.ProductId == productId)
                .OrderByDescending(x => x.SaleDate)
                .ThenByDescending(x => x.Id)
                .ToListAsync();
        }

        // =========================
        // VENTAS POR ESTADO
        // =========================


        public async Task<IEnumerable<Sale>> GetByStatusAsync(SaleStatus status)
        {
            return await GetReadQuery()
                .Where(x => x.SaleStatus == status)
                .OrderByDescending(x => x.SaleDate)
                .ThenByDescending(x => x.Id)
                .ToListAsync();
        }

        // =========================
        // VENTAS PENDIENTES DE LIQUIDAR
        // =========================
        public async Task<IEnumerable<Sale>> GetDeliveredPendingSettlementByCourierAsync(int courierCompanyId)
        {
            return await GetReadQuery()
                .Where(x =>
                    x.CourierCompanyId == courierCompanyId &&
                    x.SaleStatus == SaleStatus.Delivered &&
                    x.SettlementStatus == SettlementStatus.Pending &&
                    x.FundsLocation == FundsLocation.HeldByCourier)
                .OrderBy(x => x.SaleDate)
                .ThenBy(x => x.Id)
                .ToListAsync();
        }

        public Task<Sale?> UpdateAsync(Sale sale)
        {
            throw new NotImplementedException();
        }



        private IQueryable<Sale> GetReadQuery()
        {
            return _context.Sales
                .AsNoTracking()

                .Include(x => x.Product)
                .ThenInclude(x => x.Brand)

                .Include(x => x.CourierCompany)

                .Include(x => x.SalesChannel)

                .Include(x => x.LiquidationItem);
        }
    }
}
