using System;
using System.Transactions;

namespace API_PRIMECRM.Domain.Interfaces.Finance;

public interface IITransactionRepository
{

    public Task AddTransactionAsync(Transaction transaction);
    public Task<Transaction> GetTransactionForUpdateByIdAsync(int id);
    public Task<IEnumerable<Transaction>> GetAllTransactionsAsync();
    public Task DeleteTransactionAsync(Transaction transaction);

    
}
