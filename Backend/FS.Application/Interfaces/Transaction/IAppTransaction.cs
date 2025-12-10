namespace FS.Application.Interfaces.Transaction;

public interface IAppTransaction : IAsyncDisposable
{
    Task CommitAsync(CancellationToken ct);
}