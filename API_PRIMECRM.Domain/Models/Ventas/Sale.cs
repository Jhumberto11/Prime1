using API_PRIMECRM.Domain.Models.Inventario;
using API_PRIMECRM.Domain.Models.Liquidaciones;
using API_PRIMECRM.Domain.Models.Masters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Text;

namespace API_PRIMECRM.Domain.Models.Ventas
{
    public class Sale : TransactionsBase
    {
        public int Id { get; set; }


        // Fecha en la que se registró/agendó la venta
        public DateTime SaleDate { get; set; } = DateTime.UtcNow;


        // Producto vendido
        public int ProductId { get; set; }

        public Product Product { get; set; } = null!;


        // Cantidad vendida
        public int Quantity { get; set; }


        // Monto total cobrado al cliente
        [Column(TypeName = "decimal(18,2)")]
        public decimal SaleAmount { get; set; }


        // Costo histórico de una unidad al momento de realizar la venta
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitCostAtSale { get; set; }


        // Ubicación del cliente
        [Required]
        [MaxLength(100)]
        public string Department { get; set; } = string.Empty;


        [Required]
        [MaxLength(100)]
        public string Municipality { get; set; } = string.Empty;


        // Empresa encargada de entregar el pedido
        public int CourierCompanyId { get; set; }

        public CourierCompany CourierCompany { get; set; } = null!;


        // Costo de envío al momento de la venta
        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingCostRate { get; set; }
     


        // Comisión por manejo de efectivo
        [Column(TypeName = "decimal(18,2)")]
        public decimal CashHandlingFee { get; set; }
        // Costo Total del envio
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalShippingCost { get; set; }


        // Número de guía / tracking
        [MaxLength(100)]
        public string TrackingNumber { get; set; } = string.Empty;
        public LiquidationItem? LiquidationItem { get; set; } = null!;

        public int InventoryMovementId { get; set; }
        public InventoryMovement InventoryMovement { get; set; }


        


        // Estado general de la venta
        public SaleStatus SaleStatus { get; set; }
            = SaleStatus.Pending;


        // Estado de la liquidación
        public SettlementStatus SettlementStatus { get; set; }
            = SettlementStatus.Pending;


        // Dónde se encuentra actualmente el dinero
        public FundsLocation FundsLocation { get; set; }
            = FundsLocation.NoSettle;


        // Cómo se maneja el costo de envío
        public ShippingPaymentMode ShippingPaymentMode { get; set; }
            = ShippingPaymentMode.Cash;


        // Canal donde se realizó la venta
        public int? SalesChannelId { get; set; }

        public SalesChannel SalesChannel { get; set; } = null!;


        [MaxLength(500)]
        public string? Notes { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Profit { get; set; } = 0m;

        


 
    }
}