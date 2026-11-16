using API_PRIMECRM.Domain.Models.Finances.History;
using System;

public interface IFinanceRepository
{

    public Task CreateFinancialTransactionAsync(FinancialTransactionsHistory financialTransaction);
    public Task<FinancialTransactionsHistory> GetFinancialTransactionByIdAsync(int id);
    public Task<IEnumerable<FinancialTransactionsHistory>> GetAllFinancialTransactionsAsync();
    public Task<IEnumerable<FinancialTransactionsHistory>> GetAllFinancialTransactionsFromDateAsync(DateTime fromDate);
    


}
