using API_PRIMECRM.Domain.Models.Masters;
using System;
using System.Collections.Generic;
using System.Text;

namespace API_PRIMECRM.Domain.Models.Finances
{
    public class AdsTransactions : TransactionsBase
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public int Duration { get; set; } //Dias que durara el pago publicitario
        public decimal DailyAmount { get; set; }
        public string? Descripcion { get; set; }
        public decimal TotalAmount { get; set; }
        public bool Paid { get; set; } = false; // Para saber si se descuenta o no se descuenta del capital aun
        public ICollection<Product> Products { get; set; }



    }
}
