using FS.Application.Interfaces.Transaction;
using FS.Persistence.Context;

namespace FS.Persistence.Transactions;

public sealed class EfCoreTransactionFactory(ApplicationDbContext dbContext) : ITransactionFactory
{
    public async Task<IAppTransaction> BeginAsync(CancellationToken cancellationToken = default)
    {
        var dbTx = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        return new EfCoreAppTransaction(dbTx);
    }
}