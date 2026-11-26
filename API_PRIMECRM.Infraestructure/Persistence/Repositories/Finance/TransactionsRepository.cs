using System;
using API_PRIMECRM.Domain.Models.Finances   ;
using API_PRIMECRM.Domain.Interfaces.Finance;
using PrimeCRM_Api.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace API_PRIMECRM.Infraestructure.Persistence.Repositories;

public class TransactionsRepository : IITransactionRepository
{
    public readonly AppDbContext _context;
    public TransactionsRepository(AppDbContext context)
    {
        _context = context;
    }
    

    public async Task AddTransactionAsync(Transaction transaction)
    {
        await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTransactionAsync(Transaction transaction)
    {
        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync();
    }
    }

    public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync()
    {
        return await _context.Transactions.ToListAsync();
    }

    public async Task<Transaction> GetTransactionForUpdateByIdAsync(int id)
    {
        return await _context.Transactions.FirstOrDefaultAsync(t => t.Id == id);
    }

    
}
