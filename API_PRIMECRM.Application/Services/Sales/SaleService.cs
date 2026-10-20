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
        private readonly IUnitOfWork _unitOfWork;

        public SaleService(
            ISaleRepository saleRepository,
            IInventoryRepository inventoryRepository,
            IBaseRepository<CourierCompany> courierRepository,
            IUnitOfWork unitOfWork
            )
        {
            _courierRepository = courierRepository;
            _saleRepository = saleRepository;
            _inventoryRepository = inventoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Sale> CreateAsync(CreateSaleDto dto)
        {

            if(dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }
            if(dto.Quantity <= 0)
            {
                throw new InvalidOperationException("Cantidad no puede ser menor o igual a 0");

            }
            if (dto.SaleAmount < 0m)
            {
                throw new InvalidOperationException("Monto no puede ser menor a $0.00");

            }
            bool productExists =
            await _saleRepository.ExistProductAsync(dto.ProductId);

            if (!productExists)
            {

                throw new KeyNotFoundException($"Producto con ID : {dto.ProductId} no ha sido encontrado");
            }

            var courier = await _courierRepository
             .GetByIdAsync(dto.CourierCompanyId);

            if (courier == null)
                throw new KeyNotFoundException(
                    $"Courier con ID {dto.CourierCompanyId} no encontrado.");

            decimal unitCost = await _inventoryRepository
                .GetCurrentAverageUnitCostAsync(dto.ProductId);

            decimal cashHandlingFee = CalculateCashHandlingFee(
                                        courier.CashHandlingValue,
                                        dto.SaleAmount,
                                        dto.ShippingPaymentMode);

            decimal shippingCost = courier.DeliveryRate;

            decimal profit = CalculateProfitProduct(
            dto.SaleAmount,
            unitCost,
            dto.Quantity,
            shippingCost,
            cashHandlingFee);




            var totalCostShipping = CalculateTotalShippingSale(courier.DeliveryRate, courier.CashHandlingValue,  dto.SaleAmount, dto.ShippingPaymentMode);


          


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
                ShippingCostRate = shippingCost,
                CashHandlingFee = cashHandlingFee,
                TotalShippingCost = totalCostShipping,
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
            _unitOfWork.SaveChangesAsync();

            return newSale;
        }

        // ==========================
        // CONSULTAS
        // ==========================

        public Task<IEnumerable<Sale>> GetAllAsync()
        {
            return _saleRepository.GetAllAsync();
        }

        public async Task<Sale> GetByIdAsync(int id)
        {
            var sale = await _saleRepository.GetByIdAsync(id);

            if (sale == null)
                throw new KeyNotFoundException(
                    $"Venta con ID {id} no encontrada.");

            return sale;
        }

        public async Task<IEnumerable<Sale>> GetByProductIdAsync(
            int productId)
        {
            var sales = await _saleRepository.GetByProductIdAsync(productId);

            return sales.ToList();
        }

        public async Task<IEnumerable<Sale>> GetByStatusAsync(
            SaleStatus status)
        {
            var sales = await _saleRepository.GetAllAsync();

            return sales
                .Where(s => s.SaleStatus == status)
                .ToList();
        }

        public async Task<IEnumerable<Sale>> GetByDateRangeAsync(
            DateTime from,
            DateTime to)
        {
            if (from > to)
                throw new ArgumentException(
                    "La fecha inicial no puede superar la final.");

            

            var sales = await _saleRepository.GetByDateRangeAsync(from, to);

            return sales.ToList();
        }

        // ==========================
        // ESTADOS DE VENTA
        // ==========================

        public async Task MarkAsDeliveredAsync(int saleId)
        {
            await ChangeStatusAsync(
                saleId,
                SaleStatus.Delivered);
        }

        public async Task MarkAsLostAsync(int saleId)
        {
            await ChangeStatusAsync(
                saleId,
                SaleStatus.Lost);
        }

        public async Task MarkAsNotDeliveredAsync(int saleId)
        {
            await ChangeStatusAsync(
                saleId,
                SaleStatus.NotDelivered);
        }

        // ==========================
        // ACTUALIZAR TRACKING
        // ==========================

        public async Task UpdateTrackingAsync(
            int saleId,
            string trackingNumber)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
                throw new ArgumentException(
                    "El número de tracking es obligatorio.");

            var sale = await GetByIdAsync(saleId);

            sale.TrackingNumber = trackingNumber.Trim();

            await _unitOfWork.SaveChangesAsync();
        }


        private decimal CalculateTotalShippingSale(decimal costShipping , decimal cashHandi, decimal montSale , ShippingPaymentMode modePayment)
        {
            if(modePayment == ShippingPaymentMode.Cash)
            {
                return (costShipping + (cashHandi * montSale));
            }
            return costShipping;

        }





        // ==========================
        // MÉTODOS PRIVADOS
        // ==========================

        private async Task ChangeStatusAsync(
            int saleId,
            SaleStatus newStatus)
        {
           
            var sale = await _saleRepository.GetByIdForUpdateAsync(saleId);
            if(sale == null)
            {
                throw new KeyNotFoundException($"Venta con id {saleId} no econtrada");
            }

            sale.SaleStatus = newStatus;

            if (newStatus == SaleStatus.Delivered)
            {

                sale.InventoryMovement.OrderStatus = OrderStatus.Delivered;
            }
            if (newStatus == SaleStatus.NotDelivered) 
            {

                sale.InventoryMovement.OrderStatus = OrderStatus.Delivered;
                sale.InventoryMovement.Type = InventoryMovementType.SaleReturn;
            }
            

            await _unitOfWork.SaveChangesAsync();
            
        }

        private static decimal CalculateCashHandlingFee(
            decimal cashHandlingRate,
            decimal saleAmount,
            ShippingPaymentMode paymentMode)
        {
            if (paymentMode == ShippingPaymentMode.Cash)
                return cashHandlingRate * saleAmount;

            return 0m;
        }

        private static decimal CalculateProfitProduct(
            decimal saleAmount,
            decimal unitCost,
            int quantity,
            decimal shippingCost,
            decimal cashHandlingFee)
        {
            decimal totalProductCost = unitCost * quantity;

            return saleAmount
                - totalProductCost
                - shippingCost
                - cashHandlingFee;
        }
    }
}
