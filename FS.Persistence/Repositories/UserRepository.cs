using FS.Application.Exceptions;
using FS.Core.Entities;
using FS.Core.Stores;
using FS.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FS.Persistence.Repositories;

public class UserRepository(ApplicationDbContext context) : IUserRepository
{
    public async Task CreateAsync(User user, CancellationToken cancellationToken)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> IsEmailUnique(string email, CancellationToken cancellationToken)
    {
        return !await context.Users.Where(u => u.Email.Value == email).AnyAsync(cancellationToken);
    }

    public async Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await context.Users.Where(u => u.Email.Value == email).FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(nameof(User), email);
    }
}