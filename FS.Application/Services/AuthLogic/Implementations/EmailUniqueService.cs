using FS.Core.Services;
using FS.Core.Stores;

namespace FS.Application.Services.AuthLogic.Implementations;

public class EmailUniqueService(IUserRepository userRepository) : IEmailUniqueService
{
    public async Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken)
    {
        return await userRepository.IsEmailUnique(email, cancellationToken);
    }
}