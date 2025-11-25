namespace FS.Application.Interfaces.Transaction;

public interface ITransactionFactory
{
    Task<IAppTransaction> BeginAsync(CancellationToken ct);
}