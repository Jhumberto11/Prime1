using API_PRIMECRM.Domain.Models.Finances.Enums;
using API_PRIMECRM.Domain.Models.Ventas;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace API_PRIMECRM.Domain.Models.Finances
{
    public class FinancialTransactionsHistory
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [Required]
        public FinancialTypeTransaction TypeTransaction { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal AmountTransaction { get; set; }

        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Deducted;

        public int? TransactionId { get; set; }
        public Transaction? Transaction { get; set; }


        public int? AdsTransactionId { get; set; }
        public AdsTransactions? AdsTransaction { get; set; }


        public int? SaleId { get; set;}

        public Sale? Sale { get; set; }






    }
}
