using API_PRIMECRM.Domain.Models.Masters.Enums;
using API_PRIMECRM.Domain.Models.Reabastecimimento;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace API_PRIMECRM.Domain.Models.Masters
{
    public class PaymentMethod : Base
    {
        public PaymentMethodType Type { get; set; }

        public BankName BankName { get; set; }

        [MaxLength(4)]
        public string? Last4 { get; set; }

        public ICollection<RestockOrder> RestockOrders { get; set; }


    }
}
