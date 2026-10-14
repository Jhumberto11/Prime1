using API_PRIMECRM.Domain.DTOs.Sales;
using API_PRIMECRM.Domain.Interfaces;
using API_PRIMECRM.Domain.Interfaces.Inventory;
using API_PRIMECRM.Domain.Interfaces.Sales;
using API_PRIMECRM.Domain.Interfaces.Services;
using API_PRIMECRM.Domain.Models.Inventario;
using API_PRIMECRM.Domain.Models.Inventario.Enums;
using API_PRIMECRM.Domain.Models.Masters;
using API_PRIMECRM.Domain.Models.Reabastecimimento;
using API_PRIMECRM.Domain.Models.Ventas;
using System;
using System.Collections.Generic;
using System.Text;

namespace API_PRIMECRM.Application.Services.Sales
{
    public class SaleService : ISaleService
    {
        private readonly ISaleRepository _saleRepository
        private readonly IInventoryRepository _inventoryRepository
        private readonly IBaseRepository<CourierCompany> _courierRepository
        public SaleService(
            ISaleRepository saleRepository,
            IInventoryRepository inventoryRepository,
            IBaseRepository<CourierCompany> courierRepository
            )
        {
            _courierRepository = courierRepository,
            _saleRepository =  saleRepository,
            _inventoryRepository = inventoryRepository

        }

        public Task<Sale> CreateAsync(CreateSaleDto dto)
        {

            if(dto == null)
            {
                throw new InvalidOperationException("Venta viene nula")
            }
            if(dto.Quantity <= 0)
            {
                throw new InvalidOperationException("Cantidad no puede ser menor o igual a 0")

            }
            if (dto.SaleAmount < 0.00)
            {
                throw new InvalidOperationException("Monto no puede ser menor a $0.00")

            }
            var productExist = _saleRepository.ExistProductAsync(dto.ProductId)
            if (!productExist)
            {

                throw new KeyNotFoundException($"Producto con ID : {dto.ProductId} no ha sido encontrado")
            }

            var unitCost = GetAverageCostProduct(dto.ProductId)

            var newSale = new Sale()
            {
             
                ProductId = dto.ProductId,
                 Department = dto.Department,
                  Municipality = dto.Municipality,
                   Quantity = dto.Quantity,
                    SaleAmount = dto.SaleAmount,
                     CourierCompanyId = dto.CourierCompanyId,
                      SaleChannelId = dto.SalesChannelId,
                       Notes = dto.Notes,
                        TrackingNumber = dto.TrackingNumber,
                         UnitCostAtSale = unitCost,
                          
            }

            var newMovementSale = new InventoryMovement()
            {

                ProductID = dto.ProductId,

                Date = DateOnly.FromDateTime(
                    DateTime.Now),

                Type = InventoryMovementType.SaleOut,

                OrderStatus = OrderStatus.InTransit,

                Quantity = dto.Quantity,

                UnitCost = 

            };

            newSale.InventoryMovement = newMovementSale

            return _saleRepository.AddAsync()
        }

        public Task<IEnumerable<Sale>> GetAllAsync()
        {
            return _saleRepository.GetAllAsync();
        }

        public Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime from, DateTime to)
        {
            return Ge
        }

        public Task<Sale> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Sale>> GetByProductIdAsync(int productId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Sale>> GetByStatusAsync(SaleStatus status)
        {
            throw new NotImplementedException();
        }

        public Task MarkAsDeliveredAsync(int saleId)
        {
            throw new NotImplementedException();
        }

        public Task MarkAsLostAsync(int saleId)
        {
            throw new NotImplementedException();
        }

        public Task MarkAsNotDeliveredAsync(int saleId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateTrackingAsync(int saleId, string trackingNumber)
        {
            throw new NotImplementedException();
        }

        private decimal GetAverageCostProduct(int id)
        {
            return _inventoryRepository.GetCurrentAverageUnitCostAsync(id)
        }

        private decimal GetTotalShippingSale(decimal costShipping , decimal cashHandi)
        {
            
            
        }
    }
}
