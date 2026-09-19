using System;
using System.Collections.Generic;
using System.Text;
using API_PRIMECRM.Domain.Models.Ventas;

namespace API_PRIMECRM.Domain.Models.Masters
{
 
    /// <summary>
    /// Canales de Venta : Instagram, Facebook, etc...
    /// </summary>

    public class SalesChannel : Base
    {

        public ICollection<Sale> Sales { get; set; }
    }
}
