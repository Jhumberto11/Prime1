using API_PRIMECRM.Domain.DTOs;
using API_PRIMECRM.Domain.Interfaces;
using API_PRIMECRM.Domain.Interfaces.Master_Interfaces;
using API_PRIMECRM.Domain.Interfaces.Restock;
using API_PRIMECRM.Domain.Interfaces.Services;
using API_PRIMECRM.Domain.Models.Masters;
using API_PRIMECRM.Domain.Models.Reabastecimimento;
using API_PRIMECRM.Domain.Models.Reabastecimimento.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace API_PRIMECRM.Application.Services.Restock_Orders
{
    public class RestockOrderService : IRestockOrderService
    {
        private readonly IBaseRepository<Product> _productRepository;
        private readonly IBaseRepository<FreightCompany> _freightCompanyRepository;
        private readonly IRestockRepository _restockOrderRepository;
        private readonly IBaseRepository<PaymentMethod> _paymentMethodRepository;

        public RestockOrderService(IBaseRepository<Product> productRepository,
            IBaseRepository<FreightCompany> freightCompanyRepository,
            IRestockRepository restockOrderRepository,
            IBaseRepository<PaymentMethod> paymentMethodRepository)
        {
            _productRepository = productRepository;
            _freightCompanyRepository = freightCompanyRepository;
            _restockOrderRepository = restockOrderRepository;
            _paymentMethodRepository = paymentMethodRepository;
        }

        public async Task<RestockOrder> CreateAsync(CreateRestockOrderDto dto)
        {
            if (dto.Quantity <= 0)
                throw new ArgumentException(
                    "La cantidad debe ser mayor que cero.");

            if (dto.Total <= 0)
                throw new ArgumentException(
                    "El total debe ser mayor que cero.");

            if (dto.EstimatedPounds <= 0)
                throw new ArgumentException(
                    "El peso estimado debe ser mayor que cero.");


            // -------------------------
            // PRODUCTO
            // -------------------------

            var product = await _productRepository
                .GetByIdAsync(dto.ProductId);

            if (product == null)
                throw new KeyNotFoundException(
                    "El producto seleccionado no existe.");


            // -------------------------
            // EMPRESA DE FLETE
            // -------------------------

            var freightCompany =
                await _freightCompanyRepository
                    .GetByIdAsync(dto.FreightCompanyId);

            if (freightCompany == null)
                throw new KeyNotFoundException(
                    "La empresa de flete seleccionada no existe.");


            // -------------------------
            // MÉTODO DE PAGO
            // -------------------------

            var paymentMethod =
                await _paymentMethodRepository
                    .GetByIdAsync(dto.PaymentMethodId);

            if (paymentMethod == null)
                throw new KeyNotFoundException(
                    "El método de pago seleccionado no existe.");


            // -------------------------
            // CALCULAR FLETE
            // -------------------------

            decimal estimatedFreight =
                CalculateEstimatedFreight(
                    freightCompany,
                    dto.EstimatedPounds);


            // -------------------------
            // CALCULAR IMPUESTOS
            // -------------------------

            decimal estimatedTaxes =
                CalculateEstimatedTaxes(
                    freightCompany,
                    dto.Total);
            // -------------------------
            // CALCULAR COSTO UNIDAD
            // -------------------------

            decimal estimatedLandedUnitCost =
                CalculateEstimatedLandedUnitCost(
                dto.Total,
                (estimatedTaxes + estimatedFreight),
                dto.Quantity);
            // -------------------------
            // CALCULAR ESTIMADO TOTAL SV
            // -------------------------

            decimal estimatedTotalSV =
                CalculateTotalSV(
                dto.Total,
                estimatedFreight,
                estimatedTaxes,
                0.00m
                );


            // -------------------------
            // CREAR REABASTECIMIENTO
            // -------------------------

            var restockOrder = new RestockOrder
            {
                OrderDate = dto.OrderDate ?? DateTime.UtcNow,

                ProductId = dto.ProductId,

                Total = dto.Total,

                Quantity = dto.Quantity,

                FreightCompanyId = dto.FreightCompanyId,

                PaymentMethodId = dto.PaymentMethodId,

                EstimatedPounds = dto.EstimatedPounds,

                EstimatedFreight = estimatedFreight + estimatedTaxes,

                EstimatedTaxes = estimatedTaxes,

                EstimatedLandedUnitCost = estimatedLandedUnitCost,

                EstimatedTotalSV = estimatedTotalSV,

                PaymentStatus = PaymentStatus.Pending,

                PackageStatus = PackageStatus.InTransit,

                Notes = dto.Notes
            };


            await _restockOrderRepository
                .AddAsync(restockOrder);

            return restockOrder;
        }

        public async Task<IEnumerable<RestockOrder>> GetAllAsync()
        {
            return await _restockOrderRepository.GetAllAsync();
        }

        public Task<RestockOrder?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("El ID debe ser mayor que cero.");
            }
            var result = _restockOrderRepository.GetByIdAsync(id);
            if (result == null)
            {
                throw new KeyNotFoundException($"No se encontró un pedido de reabastecimiento con el ID {id}.");
            }


            return result;
        }

        public Task<IEnumerable<RestockOrder>> GetByProductIdAsync(int productId)
        {
            if (productId <= 0)
            {
                throw new ArgumentException("El ID debe ser mayor que cero.");
            }
            var result = _productRepository.GetByIdAsync(productId);
            if (result == null)
            {
                throw new KeyNotFoundException($"No se encontró un producto con el ID {productId}.");
            }

            return _restockOrderRepository.GetByProductIdAsync(productId);
        }



        public async Task<RestockOrder?> RegisterActualCostsAsync(int id, decimal otherCharges)
        {
            if (id <= 0)
                throw new ArgumentException(
                    "El ID debe ser mayor que cero.");

            if (otherCharges < 0)
                throw new ArgumentException(
                    "Los cargos adicionales no pueden ser negativos.");

            var restockOrder =
                await _restockOrderRepository.GetByIdAsync(id);

            if (restockOrder == null)
            {
                throw new KeyNotFoundException(
                    $"No se encontró un pedido de reabastecimiento con el ID {id}.");
            }

            restockOrder.OtherCharges = otherCharges;
            restockOrder.ActualFreight = (restockOrder.EstimatedFreight + otherCharges);

            restockOrder.ActualLandedUnitCost = CalculateLandedUnitCost(
                restockOrder.Total,
                restockOrder.ActualFreight ?? 0.00m,
                otherCharges,
                restockOrder.Quantity);

            restockOrder.ActualTotalSV = CalculateTotalSV(
                restockOrder.Total,
                restockOrder.ActualFreight ?? 0.00m,
                restockOrder.EstimatedTaxes,
                otherCharges);


            await _restockOrderRepository
                .UpdateAsync(restockOrder);

            return restockOrder;
        }

        private decimal CalculateEstimatedFreight(FreightCompany company, decimal pounds)
        {
            return ((pounds * company.RatePerLB) + company.OtherCharges);
        }


        private decimal CalculateEstimatedTaxes(FreightCompany company, decimal purchaseTotal)
        {
            return purchaseTotal *
                   (company.TaxPercentSV / 100m);
        }

        private decimal CalculateEstimatedLandedUnitCost(decimal total, decimal estimatedFreight, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException(
                    "La cantidad debe ser mayor que cero.");
            return (total + estimatedFreight) / quantity;
        }
        private decimal CalculateLandedUnitCost(decimal total, decimal actualFreight, decimal otherCharges, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException(
                    "La cantidad debe ser mayor que cero.");
            return (total + actualFreight + otherCharges) / quantity;
        }

        private decimal CalculateTotalSV(decimal total, decimal estimatedFreight, decimal estimatedTaxes, decimal? otherCharges)
        {

            return total
                + estimatedFreight
                + estimatedTaxes
                + (otherCharges ?? 0.00m);
        }

        public async Task<RestockOrder?> UpdateAsync(int id, UpdateRestockOrderDto dto)
        {
            if (id <= 0)
                throw new ArgumentException(
                    "El ID debe ser mayor que cero.");
            try
            {
                var restockOrder = await _restockOrderRepository.GetByIdAsync(id);
                if (restockOrder == null)
                    throw new KeyNotFoundException(
                        $"No se encontró un pedido de reabastecimiento con el ID {id}.");
                restockOrder.OrderDate = dto.OrderDate;
                restockOrder.ProductId = dto.ProductId;
                restockOrder.FreightCompanyId = dto.FreightCompanyId;
                restockOrder.Total = dto.Total;
                restockOrder.EstimatedPounds = dto.EstimatedPounds;
                restockOrder.Quantity = dto.Quantity;
                restockOrder.PaymentMethodId = dto.PaymentMethodId;
                restockOrder.Notes = dto.Notes;
                return await _restockOrderRepository.UpdateAsync(restockOrder);

            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar el pedido de reabastecimiento: {ex.Message}");
            }
        }
    }
}
