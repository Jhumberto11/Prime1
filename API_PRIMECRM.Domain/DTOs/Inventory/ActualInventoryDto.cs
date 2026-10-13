using System;
using System.Collections.Generic;
using System.Text;

namespace API_PRIMECRM.Domain.DTOs.Inventory
{
    public class ActualInventoryDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;

        public int TotalEntries { get; set; }
        public int TotalExits { get; set; }

        public int ActualStock { get; set; }

        public decimal? AverageRestockCost { get; set; }

        public decimal? ActualCostStock { get; set; }

        public DateOnly LastMovementDate { get; set; }
    }
}
