using API_PRIMECRM.Domain.DTOs.Restock;
using API_PRIMECRM.Domain.Interfaces;
using API_PRIMECRM.Domain.Interfaces.Inventory;
using API_PRIMECRM.Domain.Interfaces.Master_Interfaces;
using API_PRIMECRM.Domain.Interfaces.Restock;
using API_PRIMECRM.Domain.Interfaces.Services;
using API_PRIMECRM.Domain.Models.Inventario;
using API_PRIMECRM.Domain.Models.Inventario.Enums;
using API_PRIMECRM.Domain.Models.Masters;
using API_PRIMECRM.Domain.Models.Reabastecimimento;
using API_PRIMECRM.Domain.Models.Reabastecimimento.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;

namespace API_PRIMECRM.Application.Services.Restock_Orders
{
    public class RestockOrderService : IRestockOrderService
    {
        private readonly IBaseRepository<Product> _productRepository;
        private readonly IBaseRepository<FreightCompany> _freightCompanyRepository;
        private readonly IRestockRepository _restockOrderRepository;
        private readonly IBaseRepository<PaymentMethod> _paymentMethodRepository;
        private readonly IInventoryRepository _inventoryMovements;

        public RestockOrderService(IBaseRepository<Product> productRepository,
            IBaseRepository<FreightCompany> freightCompanyRepository,
            IRestockRepository restockOrderRepository,
            IBaseRepository<PaymentMethod> paymentMethodRepository,
            IInventoryRepository inventoryMovements)
        {
            _productRepository = productRepository;
            _freightCompanyRepository = freightCompanyRepository;
            _restockOrderRepository = restockOrderRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _inventoryMovements = inventoryMovements;
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
                estimatedFreight,
                estimatedTaxes,
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

                EstimatedFreight = estimatedFreight ,

                EstimatedTotalFreightAndTaxes = estimatedFreight + estimatedTaxes,

                EstimatedTaxes = estimatedTaxes,

                EstimatedLandedUnitCost = estimatedLandedUnitCost,

                EstimatedTotalSV = estimatedTotalSV,

                PaymentStatus = PaymentStatus.Pending,

                PackageStatus = PackageStatus.InTransit,

                Notes = dto.Notes
            };

            // -------------------------
            // CREAR MOVIMIENTO INVENTARIO
            // -------------------------

            var newInventoryMovement = new InventoryMovement
            {
                ProductID = restockOrder.ProductId,

                Date = DateOnly.FromDateTime(
                    restockOrder.OrderDate),

                Type = InventoryMovementType.RestockIn,

                OrderStatus = OrderStatus.InTransit,

                Quantity = restockOrder.Quantity,

                UnitCost = restockOrder.EstimatedLandedUnitCost
            };

            // -------------------------
            // ASOCIAR MOVIMIENTO A RESTOCK
            // -------------------------


            restockOrder.InventoryMovement = newInventoryMovement;


            await _restockOrderRepository
                .AddAsync(restockOrder);

            return restockOrder;
        }

        public async Task<IEnumerable<RestockOrder>> GetAllAsync()
        {
            return await _restockOrderRepository.GetAllAsync();
        }

        /// <summary>
        /// Regresa Reabastecimento por ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        public async Task<RestockOrder?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("El ID debe ser mayor que cero.");
            }
            var result =  await _restockOrderRepository.GetByIdAsync(id);
            if (result == null)
            {
                throw new KeyNotFoundException($"No se encontró un pedido de reabastecimiento con el ID {id}.");
            }


            return result;
        }


        /// <summary>
        /// Regresa lista de reabastecimentos por producto
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
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



        /// <summary>
        /// Registra los costos reales de un pedido de reabastecimiento, incluyendo cargos adicionales y recalcula el costo por unidad y el total en El Salvador
        /// </summary>
        /// <param name="id"></param>
        /// <param name="otherCharges"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        public async Task<RestockOrder?> RegisterActualCostsAsync(int id, decimal otherCharges)
        {
            if (id <= 0)
                throw new ArgumentException(
                    "El ID debe ser mayor que cero.");

            if (otherCharges < 0)
                throw new ArgumentException(
                    "Los cargos adicionales no pueden ser negativos.");

            var restockOrder =
                await _restockOrderRepository.GetByIdForUpdateAsync(id);

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

            restockOrder.EstimatedLandedUnitCost = CalculateLandedUnitCost(
                restockOrder.Total,
                restockOrder.ActualFreight ?? 0.00m,
                otherCharges,
                restockOrder.Quantity);



            await _restockOrderRepository
                .UpdateAsync(restockOrder);

            return restockOrder;
        }



        public async Task<RestockOrder?> UpdateAsync(int id, UpdateRestockOrderDto dto)
        {
            if (id <= 0)
                throw new ArgumentException(
                    "El ID del Restock debe ser mayor que cero.");


            // VALIDAR PRODUCTO
            var product = await _productRepository.GetByIdAsync(dto.ProductId);

             if(product == null)
                 throw new KeyNotFoundException(
                     $"No se encontró un producto con el ID {dto.ProductId}.");

            // VALIDAR EMPRESA DE FLETE
            var freightCompany = await _freightCompanyRepository.GetByIdAsync(dto.FreightCompanyId);

             if(freightCompany == null)
                 throw new KeyNotFoundException(
                     $"No se encontró una empresa de flete con el ID {dto.FreightCompanyId}.");

            // VALIDAR MÉTODO DE PAGO
            var paymentMethod = await _paymentMethodRepository
                .GetByIdAsync(dto.PaymentMethodId);

            if (paymentMethod == null)
                throw new KeyNotFoundException(
                    "El método de pago no existe.");

            //Validaciones DTO
            if (dto.Quantity <= 0)
                throw new ArgumentException(
                    "La cantidad debe ser mayor que cero.");

            if (dto.Total <= 0)
                throw new ArgumentException(
                    "El total debe ser mayor que cero.");

            if (dto.EstimatedPounds <= 0)
                throw new ArgumentException(
                    "El peso estimado debe ser mayor que cero.");



            var restockOrder = await _restockOrderRepository.GetByIdForUpdateAsync(id);


             if (restockOrder == null)
                 throw new KeyNotFoundException(
                     $"No se encontró un pedido de reabastecimiento con el ID {id}.");



            // CALCULAR COSTOS
            decimal estimatedFreight =
                CalculateEstimatedFreight(
                    freightCompany,
                    dto.EstimatedPounds);

            decimal estimatedTaxes =
                CalculateEstimatedTaxes(
                    freightCompany,
                    dto.Total);

            decimal landedUnitCost =
                CalculateEstimatedLandedUnitCost(
                    dto.Total,
                    estimatedFreight,
                    estimatedTaxes,
                    dto.Quantity);

            decimal estimatedTotalSV =
                CalculateTotalSV(
                    dto.Total,
                    estimatedFreight,
                    estimatedTaxes,
                    0m);


            // ACTUALIZAR RESTOCK
            restockOrder.OrderDate = dto.OrderDate;
            restockOrder.ProductId = dto.ProductId;
            restockOrder.FreightCompanyId = dto.FreightCompanyId;
            restockOrder.PaymentMethodId = dto.PaymentMethodId;
            restockOrder.Total = dto.Total;
            restockOrder.EstimatedPounds = dto.EstimatedPounds;
            restockOrder.Quantity = dto.Quantity;
            restockOrder.Notes = dto.Notes;

            restockOrder.EstimatedFreight = estimatedFreight;
            restockOrder.EstimatedTaxes = estimatedTaxes;

            restockOrder.EstimatedTotalFreightAndTaxes =
                estimatedFreight + estimatedTaxes;

            restockOrder.EstimatedLandedUnitCost =
                landedUnitCost;

            restockOrder.EstimatedTotalSV =
                estimatedTotalSV;


            // ACTUALIZAR MOVIMIENTO DE INVENTARIO
            var inventoryMovement =
                restockOrder.InventoryMovement
                ?? throw new InvalidOperationException(
                    "El reabastecimiento no tiene un movimiento asociado.");

            inventoryMovement.ProductID =
                restockOrder.ProductId;

            inventoryMovement.Date =
                DateOnly.FromDateTime(restockOrder.OrderDate);

            inventoryMovement.Quantity =
                restockOrder.Quantity;

            inventoryMovement.UnitCost =
                restockOrder.EstimatedLandedUnitCost;


            return await _restockOrderRepository.UpdateAsync(restockOrder);

            
            
        }
        /// <summary>
        /// Calcula el flete estimado basado en la empresa de flete y el peso en libras
        /// </summary>
        /// <param name="company"></param>
        /// <param name="pounds"></param>
        /// <returns></returns>

        private decimal CalculateEstimatedFreight(FreightCompany company, decimal pounds)
        {
            return ((pounds * company.RatePerLB) + company.OtherCharges);
        }

        /// <summary>
        /// Calcula los impuestos estimados basado en la empresa de flete y el total de la compra   
        /// </summary>
        /// <param name="company"></param>
        /// <param name="purchaseTotal"></param>
        /// <returns></returns>

        private decimal CalculateEstimatedTaxes(FreightCompany company, decimal purchaseTotal)
        {
            return purchaseTotal *
                   (company.TaxPercentSV / 100m);
        }
        /// <summary>
        /// Calcula el costo estimado por unidad ya puesta en El Salvador
        /// </summary>
        /// <param name="total"></param>
        /// <param name="estimatedFreight"></param>
        /// <param name="estimatedTaxes"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private decimal CalculateEstimatedLandedUnitCost(decimal total, decimal estimatedFreight, decimal estimatedTaxes, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException(
                    "La cantidad debe ser mayor que cero.");
            return (total + estimatedFreight + estimatedTaxes) / quantity;
        }



        //
        //
        // Calculo de Costos Reales
        //
        // 

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


        /// <summary>
        /// Cambia estado del pedido a entregado
        /// </summary>
        /// <param name="id"></param>
        /// <param name="delivered"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<RestockOrder?> ChangeStatusDeliverded(int id, bool delivered)
        {

            // VALIDAR ID
            if (id <= 0)
                throw new ArgumentException(
                    "El ID del Restock debe ser mayor que cero.");

            // BUSCAR ORDEN
            var orderDetail = await _restockOrderRepository
                .GetByIdForUpdateAsync(id);

            if (orderDetail == null)
                throw new KeyNotFoundException(
                    $"Orden con ID {id} no encontrada.");

            // VALIDAR ESTADO ACTUAL
            if (orderDetail.PackageStatus != PackageStatus.InTransit)
                throw new InvalidOperationException(
                    $"No se puede cambiar el estado de una orden " +
                    $"que ya está en {orderDetail.PackageStatus}.");

            // VALIDAR MOVIMIENTO DE INVENTARIO
            var inventoryMovement = orderDetail.InventoryMovement;

            if (inventoryMovement == null)
                throw new InvalidOperationException(
                    "La orden no tiene un movimiento de inventario asociado.");

            // CAMBIAR ESTADO SEGÚN RESULTADO
            if (delivered)
            {
                orderDetail.PackageStatus = PackageStatus.Delivered;
                inventoryMovement.OrderStatus = OrderStatus.Delivered;
            }
            else
            {
                orderDetail.PackageStatus = PackageStatus.Lossed;
                inventoryMovement.OrderStatus = OrderStatus.Lossed;
            }

            // GUARDAR AMBOS CAMBIOS
            await _restockOrderRepository.UpdateAsync(orderDetail);

            return orderDetail;


        }

        public Task<RestockOrder?> ChangeStatusPayments(int id, bool paid)
        {
            throw new NotImplementedException();
        }

        public async Task<RestockOrder?> RevertStatusDeliveryAsync(int id)
        {
            // VALIDAR ID
            if (id <= 0)
                throw new ArgumentException(
                    "El ID del Restock debe ser mayor que cero.");

            // OBTENER ORDEN Y MOVIMIENTO
            var orderDetail = await _restockOrderRepository
                .GetByIdForUpdateAsync(id);

            if (orderDetail == null)
                throw new KeyNotFoundException(
                    $"No se encontró la orden con ID {id}.");

            // VERIFICAR QUE ESTÉ ENTREGADA
            if (orderDetail.PackageStatus != PackageStatus.Delivered)
                throw new InvalidOperationException(
                    "Solo se pueden revertir órdenes entregadas.");

            // VERIFICAR MOVIMIENTO
            var inventoryMovement = orderDetail.InventoryMovement;

            if (inventoryMovement == null)
                throw new InvalidOperationException(
                    "La orden no tiene movimiento de inventario.");

            if (inventoryMovement.OrderStatus != OrderStatus.Delivered)
                throw new InvalidOperationException(
                    "El movimiento no figura como entregado.");

            // IMPORTANTE:
            // Antes de revertir, comprobar que las unidades
            // no estén comprometidas por ventas o reservas.
            // Esta validación debe implementarse según
            // cómo calcules y almacenes el stock.

            // REVERTIR ESTADOS
            orderDetail.PackageStatus = PackageStatus.InTransit;

            inventoryMovement.OrderStatus = OrderStatus.InTransit;

            // GUARDAR CAMBIOS
            await _restockOrderRepository.UpdateAsync(orderDetail);

            return orderDetail;
        }
    }
}
