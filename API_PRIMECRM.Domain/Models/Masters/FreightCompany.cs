using API_PRIMECRM.Domain.Models.Reabastecimimento;
using System;
using System.Collections.Generic;
using System.Text;

namespace API_PRIMECRM.Domain.Models.Masters
{
    public class FreightCompany : Base
    {
        public decimal RatePerLB { get; set; } /// Precio por Libra
        public decimal TaxPercentSV { get; set; } /// Impuestos de el salvador
        public decimal OtherCharges {  get; set; } /// Cargos Adiccionales

        public ICollection<RestockOrder> RestockOrders { get; set; } 

    }
}
