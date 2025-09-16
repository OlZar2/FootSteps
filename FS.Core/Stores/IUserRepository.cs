using FS.Core.Entities;

namespace FS.Core.Stores;

public interface IUserRepository
{
    Task CreateAsync(User user, CancellationToken cancellationToken);
    
    Task<bool> IsEmailUnique(string email, CancellationToken cancellationToken);

    Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken);
    
    Task<User> GetByIdWithContactsAsync(Guid id, CancellationToken cancellationToken);
    
    Task<User> GetByIdWithAvatarAsync(Guid id, CancellationToken cancellationToken);
    
    Task UpdateAsync(User user, CancellationToken cancellationToken);
}