using API_PRIMECRM.Domain.Models.Finances.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace API_PRIMECRM.Domain.Models.Finances
{
    public class Transaction : TransactionsBase
    {

        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string Descriptions { get; set; }
        public CategoryTransaction Category { get; set; }
        public decimal Amount { get; set; }
        public TypeTransaction TypeTransaction { get; set; }


    }
}
