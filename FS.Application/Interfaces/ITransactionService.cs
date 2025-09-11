namespace FS.Application.Interfaces;

public interface ITransactionService
{
    Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken ct);
    Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action, CancellationToken ct);
}