using System;
using System.Collections.Generic;
using System.Text;

namespace API_PRIMECRM.Domain.DTOs.Restock
{
    public class ActualCostRestockUpdate
    {
        public int RestockOrderId { get; set; }
        public decimal OtherCharges { get; set; }
    }
}
