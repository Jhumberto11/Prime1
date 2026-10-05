using API_PRIMECRM.Domain.Models.Reabastecimimento;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace API_PRIMECRM.Domain.Models.Masters
{
    public class FreightCompany : Base
    {
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal RatePerLB { get; set; } /// Precio por Libra 

        [Required]
        [Column(TypeName = "decimal(18, 4)")]
        public decimal TaxPercentSV { get; set; } /// Impuestos de el salvador

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal OtherCharges {  get; set; } /// Cargos Adiccionales

        public ICollection<RestockOrder> RestockOrders { get; set; } 

    }
}
