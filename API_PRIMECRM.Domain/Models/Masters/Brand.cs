using System;
using System.Collections.Generic;
using System.Text;

namespace API_PRIMECRM.Domain.Models.Masters
{

    public class Brand : Base
    {
        public ICollection<Product> Products { get; set; } = new List<Product>();               

    }
}
