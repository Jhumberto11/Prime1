using API_PRIMECRM.Domain.Models.Masters.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace API_PRIMECRM.Domain.DTOs.Masters
{
    public class CourierDto
    {

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal DeliveryRate { get; set; } // COSTO DE ENVIO


        [Column(TypeName = "decimal(18,4)")]
        public decimal CashHandlingValue { get; set; } // Tarifa de Comision de efectivo
    }
}
