using API_PRIMECRM.Domain.Models.Finances;
using System;
using System.Collections.Generic;
using System.Text;

namespace API_PRIMECRM.Domain.Models
{
    public class TransactionsBase
    {
        public int FinancialTransactionsHistoryId { get; set; }
        public FinancialTransactionsHistory FinancialTransactionsHistory { get; set; } = new FinancialTransactionsHistory();


    }
}
