using System;
using API_PRIMECRM.Domain.Models.Finances.Enums;
using API_PRIMECRM.Domain.Models.Finances.History;
using Microsoft.EntityFrameworkCore;
using PrimeCRM_Api.Infraestructure.Persistence;

namespace API_PRIMECRM.Infraestructure.Persistence.Repositories.Finance;

public class FinanceRepository : IFinanceRepository
{
    private readonly AppDbContext _context;

    public FinanceRepository(AppDbContext context)
    {
        _context = context;
    }
    

    public async Task CreateFinancialTransactionAsync(FinancialTransactionsHistory financialTransaction)
    {
        await _context.FinancialTransactionsHistory.AddAsync(financialTransaction);
        
    }

    public async Task<IEnumerable<FinancialTransactionsHistory>> GetAllFinancialTransactionsAsync()
    {
        return  await _context.FinancialTransactionsHistory.AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<FinancialTransactionsHistory>> GetAllFinancialTransactionsFromDateAsync(DateTime fromDate)
    {
        return await _context.FinancialTransactionsHistory.AsNoTracking()
            .Where(x => x.Date >= fromDate)
            .ToListAsync();
    }

    public async Task<FinancialTransactionsHistory> GetFinancialTransactionByIdAsync(int id)
    {
        return await _context.FinancialTransactionsHistory.AsNoTracking()
        .Include(x => x.Sale)
        .Include(x => x.AdsTransaction)
        .Include(x => x.Transaction).
        FirstOrDefaultAsync(x => x.Id == id) 
        ?? throw new KeyNotFoundException($"Financial transaction with ID {id} not found.");
        
           
    }

    public async Task DeleteFinancialTransactionAsync(FinancialTransactionsHistory financialTransaction)
    {
        _context.FinancialTransactionsHistory.Remove(financialTransaction);
    }
    

    public async Task<decimal> GetCapitalBalanceAsync()
    {
        return await _context.FinancialTransactionsHistory.AsNoTracking().Where(x => x.PaymentStatus == Status.Deducted || x.PaymentStatus == Status.Applied)
            .SumAsync(x => x.AmountTransaction);
    }
s
}
