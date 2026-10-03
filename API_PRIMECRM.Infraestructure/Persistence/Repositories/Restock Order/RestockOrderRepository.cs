using API_PRIMECRM.Domain.Interfaces.Restock;
using API_PRIMECRM.Domain.Models.Reabastecimimento;
using Microsoft.EntityFrameworkCore;
using PrimeCRM_Api.Infraestructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace API_PRIMECRM.Infraestructure.Persistence.Repositories.Restock_Order
{
    public class RestockOrderRepository : IRestockRepository
    {
        private readonly AppDbContext _context;
        public RestockOrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(RestockOrder restockOrder)
        {
            await _context.RestockOrders.AddAsync(restockOrder);
            await _context.SaveChangesAsync();
            
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.RestockOrders.AnyAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<RestockOrder>> GetAllAsync()
        {
            return await _context.RestockOrders.AsNoTracking()
                .Include(r=> r.Product)
                .Include(r => r.FreightCompany)
                .Include(r => r.PaymentMethod)
                .OrderByDescending(r => r.OrderDate)
                .ToListAsync();
        }

        public async Task<RestockOrder?> GetByIdAsync(int id)
        {
            return await _context.RestockOrders.AsNoTracking()
                .Include(r => r.Product)
                .Include(r => r.FreightCompany)
                .Include(r => r.PaymentMethod)
                .FirstOrDefaultAsync(r => r.Id == id);
        }


        /// <summary>
        /// Obtiene todas las ordenes de reabastecimiento asociadas a un producto específico por su ID.
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<RestockOrder>> GetByProductIdAsync(int productId)
        {
            return await _context.RestockOrders.AsNoTracking()
                .Include(r => r.Product)
                .Include(r => r.FreightCompany)
                .Include(r => r.PaymentMethod)
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.OrderDate)
                .ToListAsync();
        }

        public async Task UpdateAsync(RestockOrder restockOrder)
        {
            _context.RestockOrders.Update(restockOrder);

            await _context.SaveChangesAsync();
        }
    }
}
