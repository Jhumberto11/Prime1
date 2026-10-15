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
        private readonly ISaleRepository _saleRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IBaseRepository<CourierCompany> _courierRepository;
        public SaleService(
            ISaleRepository saleRepository,
            IInventoryRepository inventoryRepository,
            IBaseRepository<CourierCompany> courierRepository
            )
        {
            _courierRepository = courierRepository;
            _saleRepository = saleRepository;
            _inventoryRepository = inventoryRepository;

        }

        public async Task<Sale> CreateAsync(CreateSaleDto dto)
        {

            if(dto == null)
            {
                throw new InvalidOperationException("Venta viene nula");
            }
            if(dto.Quantity <= 0)
            {
                throw new InvalidOperationException("Cantidad no puede ser menor o igual a 0");

            }
            if (dto.SaleAmount < 0m)
            {
                throw new InvalidOperationException("Monto no puede ser menor a $0.00");

            }
            var productExist = _saleRepository.ExistProductAsync(dto.ProductId);
            if (!productExist.Result)
            {

                throw new KeyNotFoundException($"Producto con ID : {dto.ProductId} no ha sido encontrado");
            }

            var unitCost = GetAverageCostProduct(dto.ProductId);
            var courier = _courierRepository.GetByIdAsync(dto.CourierCompanyId).Result;
            if(courier == null)
            {
                throw new KeyNotFoundException($"Courier con ID : {dto.CourierCompanyId} no ha sido encontrado");
            }



            var totalCostShipping = CalculateTotalShippingSale(courier.DeliveryRate, courier.CashHandlingValue,  dto.SaleAmount, dto.ShippingPaymentMode);


            var profit = CalculateProfitProduct(totalCostShipping, dto.SaleAmount, unitCost);


            var newSale = new Sale()
            {

                ProductId = dto.ProductId,
                UnitCostAtSale = unitCost,
                Department = dto.Department,
                Municipality = dto.Municipality,
                Quantity = dto.Quantity,
                SaleAmount = dto.SaleAmount,
                CourierCompanyId = dto.CourierCompanyId,
                SalesChannelId = dto.SalesChannelId,
                Notes = dto.Notes,
                TrackingNumber = dto.TrackingNumber,
                ShippingCost = totalCostShipping,
                CashHandlingFee = (courier.CashHandlingValue * dto.SaleAmount),
                Profit = profit,
                ShippingPaymentMode = dto.ShippingPaymentMode,
                SaleStatus = dto.SaleStatus,
                FundsLocation = dto.FundsLocation
            };

            var newMovementSale = new InventoryMovement()
            {

                ProductID = dto.ProductId,

                Date = DateOnly.FromDateTime(
                    DateTime.Now),

                Type = InventoryMovementType.SaleOut,

                OrderStatus = OrderStatus.InTransit,

                Quantity = dto.Quantity,

                UnitCost = unitCost

            };

            newSale.InventoryMovement = newMovementSale;

            await _saleRepository.AddAsync(newSale);

            return newSale;
        }

        public Task<IEnumerable<Sale>> GetAllAsync()
        {
            return _saleRepository.GetAllAsync();
        }

        public Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime from, DateTime to)
        {
            throw new NotImplementedException();
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
            var averageCost = _inventoryRepository.GetCurrentAverageUnitCostAsync(id);

            return averageCost.Result;
        }

        private decimal CalculateTotalShippingSale(decimal costShipping , decimal cashHandi, decimal montSale , ShippingPaymentMode modePayment)
        {
            if(modePayment == ShippingPaymentMode.Cash)
            {
                return (costShipping + (cashHandi * montSale));
            }
            return costShipping;


            
        }

        private decimal CalculateProfitProduct(decimal costShipping , decimal montSale, decimal unitCost)
        {
            return unitCost - montSale - costShipping;
        }
    }
}
