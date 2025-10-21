using FS.Application.DTOs.AuthDTOs;
using FS.Application.Exceptions;
using FS.Application.Interfaces.QueryServices;
using FS.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FS.Persistence.QueryServices;

public class EFUserQueryService(ApplicationDbContext context) : IUserQueryService
{
    public async Task<MeInfo> GetUserMainInfoByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await context.Users
            .Include(u => u.AvatarImage)
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new MeInfo
            {
                FirstName = u.FullName.FirstName,
                SecondName = u.FullName.SecondName,
                Patronymic = u.FullName.Patronymic,
                AvatarPath = u.AvatarImage != null ? u.AvatarImage.Path : null,
            })
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException("User", id);
        
        return result;
    }
}
