using FS.Application.DTOs.AuthDTOs;
using FS.Application.Exceptions;
using FS.Application.Interfaces.QueryServices;
using FS.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FS.Persistence.QueryServices;

public class EFUserQueryService(ApplicationDbContext context) : IUserQueryService
{
    public async Task<UserMainInfo> GetUserMainInfoByIdAsync(Guid id, CancellationToken ct)
    {
        var result =
            await (
                    from u in context.Users
                    where u.Id == id
                    select new UserMainInfo
                    {
                        Id = u.Id,
                        FirstName = u.FullName.FirstName,
                        SecondName = u.FullName.SecondName,
                        Patronymic = u.FullName.Patronymic,
                        AvatarPath = u.AvatarImage == null ? null : u.AvatarImage.FullImagePath,
                        Contacts = u.Contacts
                            .Select(c => new MeContactData
                            {
                                ContactType = c.Type,
                                Url = c.Url,
                            })
                            .ToArray(),
                        Description = u.Description,
                    }
                )
                .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException("User", id);

        return result;
    }
}
