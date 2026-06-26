using FS.Application.Shared.Exceptions;
using FS.Core.UserDomain;
using FS.Core.UserDomain.Stores;
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

    public async Task<User> GetByIdWithContactsAsync(Guid id, CancellationToken ct)
    {
        return await context.Users
               .Include(u => u.Contacts)
               .FirstOrDefaultAsync(u => u.Id == id, ct)
               ?? throw new NotFoundException(nameof(User), id);
    }
    
    public async Task<User> GetByIdWithDevicesAsync(Guid id, CancellationToken ct)
    {
        return await context.Users
                   .Include(u => u.UserDevices)
                   .FirstOrDefaultAsync(u => u.Id == id, ct)
               ?? throw new NotFoundException(nameof(User), id);
    }

    public async Task<User> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Users
                   .FirstOrDefaultAsync(u => u.Id == id, cancellationToken)
               ?? throw new NotFoundException(nameof(User), id);
    }
    
    public async Task<User> GetByIdWithAvatarAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Users
                   .Include(u => u.AvatarImage)
                   .FirstOrDefaultAsync(u => u.Id == id, cancellationToken)
               ?? throw new NotFoundException(nameof(User), id);
    }

    public async Task<bool> IsEmailUnique(string email, CancellationToken cancellationToken)
    {
        return !await context.Users.Where(u => u.Email.Value == email).AnyAsync(cancellationToken);
    }

    public async Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await context.Users
                   .Where(u => u.Email.Value == email)
                   .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(nameof(User), email);
    }

    public async Task<User> GetByEmailForUpdateAsync(string email, CancellationToken cancellationToken)
    {
        var users = await context.Users
            .FromSqlInterpolated($"""
                                  SELECT *
                                  FROM "Users"
                                  WHERE "Email" = {email}
                                  LIMIT 1
                                  FOR UPDATE
                                  """)
            .ToArrayAsync(cancellationToken);

        return users.FirstOrDefault()
               ?? throw new NotFoundException(nameof(User), email);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<User[]> GetByActiveDeviceTokenWithDevicesAsync(string deviceToken, CancellationToken cancellationToken)
    {
        return await context.Users
            .Where(u => u.UserDevices.Any(ud =>
                ud.DeviceToken == deviceToken &&
                ud.IsActive))
            .Include(u => u.UserDevices.Where(ud => ud.IsActive))
            .ToArrayAsync(cancellationToken);
    }
}
