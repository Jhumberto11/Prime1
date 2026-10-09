using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace API_PRIMECRM.Domain.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync(
            IsolationLevel isolationLevel =
                IsolationLevel.ReadCommitted);

        Task CommitTransactionAsync();

        Task RollbackTransactionAsync();
    }
}
