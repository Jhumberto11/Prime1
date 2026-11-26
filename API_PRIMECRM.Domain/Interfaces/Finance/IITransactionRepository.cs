using System;
using API_PRIMECRM.Domain.Models.Finances;

namespace API_PRIMECRM.Domain.Interfaces.Finance;

public interface IITransactionRepository
{

    public Task AddTransactionAsync(Transaction transaction);
    public Task<Transaction> GetTransactionForUpdateByIdAsync(int id);
    public Task<IEnumerable<Transaction>> GetAllTransactionsAsync();
    public Task DeleteTransactionAsync(Transaction transaction);

    
}
