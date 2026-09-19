using System;
using System.Collections.Generic;
using System.Text;
using API_PRIMECRM.Domain.Models.Inventario;
using API_PRIMECRM.Domain.Models.Reabastecimimento;
using API_PRIMECRM.Domain.Models.Ventas;

namespace API_PRIMECRM.Domain.Models.Masters
{
    public class Product : Base
    {
        public Brand Brand { get; set; }
        public int BrandId { get; set; }

        public ICollection<InventoryMovement> InventoryMovements { get; set; }
        public ICollection<RestockOrder> RestockOrders { get; set; }
        public ICollection<Sale> Sales { get; set; }

    }
}
