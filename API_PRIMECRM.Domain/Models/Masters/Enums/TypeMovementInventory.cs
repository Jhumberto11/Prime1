using System;
using System.Collections.Generic;
using System.Text;

namespace API_PRIMECRM.Domain.Models.Masters.Enums
{
    public enum TypeMovementInventory
    {
        RestockIn = 1,
        SaleOut = 2,
        SaleReturn = 3,
        AdjustmentIn = 4,
        AdjustmentOut = 5
    }
}
