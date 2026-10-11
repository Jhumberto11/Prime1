using API_PRIMECRM.Domain.Models.Ventas;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace API_PRIMECRM.Domain.DTOs.Sales
{
    public class CreateSaleDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal SaleAmount { get; set; }

        [Required]
        [MaxLength(100)]
        public string Department { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Municipality { get; set; } = string.Empty;

        [Required]
        public int CourierCompanyId { get; set; }

        [MaxLength(100)]
        public string? TrackingNumber { get; set; }

        [Required]
        public int SalesChannelId { get; set; }

        //public ShippingPaymentMode ShippingPaymentMode { get; set; }
        //    = ShippingPaymentMode.DeductFromSettlement;

        //public FundsLocation FundsLocation { get; set; }
        //    = FundsLocation.HeldByCourier;

        [MaxLength(500)]
        public string? Notes { get; set; }

        // Opcional para poder registrar ventas históricas.
        // Si viene null, el servicio usa DateTime.UtcNow.

        //public DateOnly? SaleDate { get; set; }       <===================
    }
}
