using API_PRIMECRM.Domain.Models.Masters.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace API_PRIMECRM.Domain.DTOs.Masters
{
    public class FreightCompanyDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = "No Especificado"; /// Nombre de la compañia

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal RatePerLB { get; set; } /// Precio por Libra

        [Column(TypeName = "decimal(18, 4)")]
        [Required]
        public decimal TaxPercentSV { get; set; } /// Impuestos de el salvador
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal OtherCharges { get; set; } /// Cargos Adiccionales
    }
}
