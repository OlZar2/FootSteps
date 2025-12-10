using FS.Application.Interfaces.Transaction;
using Microsoft.EntityFrameworkCore.Storage;

namespace FS.Persistence.Transactions;

public class EfCoreAppTransaction(IDbContextTransaction dbTransaction) : IAppTransaction
{
    private bool _committed;

    public async Task CommitAsync(CancellationToken ct)
    {
        if (_committed) return;
        await dbTransaction.CommitAsync(ct);
        _committed = true;
    }

    public async ValueTask DisposeAsync()
    {
        if (!_committed)
        {
            await dbTransaction.RollbackAsync();
        }

        await dbTransaction.DisposeAsync();
    }
}