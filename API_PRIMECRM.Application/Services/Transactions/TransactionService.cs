using System;
using API_PRIMECRM.Domain.Models.Finances;
using API_PRIMECRM.Domain.Interfaces.Services;

namespace API_PRIMECRM.Application.Services.Transactions;

public class TransactionService : ITransactionService
{
    public TransactionService()
    {
    }

    public Task AddTransactionAsync(Transaction transaction)
    {
        throw new NotImplementedException();
    }

    public Task DeleteTransactionAsync(Transaction transaction)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Transaction>> GetAllTransactionsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Transaction> GetTransactionByIdAsync(int id)
    {
        throw new NotImplementedException();
    }
}
