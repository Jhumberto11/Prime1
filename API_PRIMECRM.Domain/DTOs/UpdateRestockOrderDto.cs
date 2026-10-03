using System;
using System.Collections.Generic;
using System.Text;

namespace API_PRIMECRM.Application.Services.Restock_Orders
{
    public class UpdateRestockOrderDto
    {
        public int ProductId { get; set; }
        public int FreightCompanyId { get; set; }
        public decimal Total { get; set; }
        public decimal EstimatedPounds { get; set; }
        public DateTime OrderDate { get; set; }
        public int Quantity { get; set; }
        public int PaymentMethodId { get; set; }
        public string? Notes { get; set; }

    }
}
