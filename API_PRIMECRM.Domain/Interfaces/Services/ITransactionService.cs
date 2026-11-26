using System;
using System.Collections.Generic;
using System.Text;
using API_PRIMECRM.Domain.Models.Finances;

namespace API_PRIMECRM.Domain.Interfaces.Services
{
    public interface ITransactionService
    {
        public Task AddTransactionAsync(Transaction transaction);
        public Task<Transaction> GetTransactionByIdAsync(int id);
        public Task<IEnumerable<Transaction>> GetAllTransactionsAsync();
        public Task DeleteTransactionAsync(Transaction transaction);
  
    }
}
