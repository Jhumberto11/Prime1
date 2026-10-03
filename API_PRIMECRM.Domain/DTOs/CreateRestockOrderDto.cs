using System;
using System.Collections.Generic;
using System.Text;

namespace API_PRIMECRM.Domain.DTOs
{
    public class CreateRestockOrderDto
    {
        public DateTime? OrderDate { get; set; }

        public int ProductId { get; set; }

        public decimal Total { get; set; }

        public int Quantity { get; set; }

        public int FreightCompanyId { get; set; }

        public int PaymentMethodId { get; set; }

        public decimal EstimatedPounds { get; set; }

        public decimal OtherCharges { get; set; }

        public string? Notes { get; set; }
    }
}
