using FS.Core.Entities;

namespace FS.Core.Stores;

public interface IUserRepository
{
    Task CreateAsync(User user, CancellationToken cancellationToken);
    
    Task<bool> IsEmailUnique(string email, CancellationToken cancellationToken);
}