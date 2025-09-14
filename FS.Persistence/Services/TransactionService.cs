using FS.Application.Interfaces;
using FS.Persistence.Context;
using Microsoft.EntityFrameworkCore.Storage;

namespace FS.Persistence.Services;

public class TransactionService(ApplicationDbContext dbContext) : ITransactionService
{
    public async Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken ct)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);
        try
        {
            await action();
            await dbContext.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action, CancellationToken ct)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);
        try
        {
            var result = await action();
            await dbContext.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
            return result;
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }
}