using API_PRIMECRM.Domain.Models.Inventario.Enums;
using API_PRIMECRM.Domain.Models.Masters;
using API_PRIMECRM.Domain.Models.Reabastecimimento;
using API_PRIMECRM.Domain.Models.Ventas;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace API_PRIMECRM.Domain.Models.Inventario
{
    public class InventoryMovement
    {
        public int Id { get; set; }

        public int ProductID { get; set; }
        // Navigation Property
        public Product Product { get; set; } = null!;


        public DateOnly Date { get; set; }
        public InventoryMovementType Type { get; set; } // Tipo de movimiento de productos
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitCost { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        // Navigation Property

        public OrderStatus OrderStatus { get; set; } = OrderStatus.InTransit;

        public int? RestockOrderId { get; set; }
        public RestockOrder? RestockOrder { get; set; }

        public int? SaleId { get; set; }
        public Sale? Sale { get; set; }

        


    }
}
