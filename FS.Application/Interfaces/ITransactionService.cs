namespace FS.Application.Interfaces;

public interface ITransactionService
{
    //TODO: удаление из S3 при неудачной транзакции
    Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken ct);
    Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action, CancellationToken ct);
}