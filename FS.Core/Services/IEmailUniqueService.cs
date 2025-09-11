namespace FS.Core.Services;

public interface IEmailUniqueService
{
    Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken);
}