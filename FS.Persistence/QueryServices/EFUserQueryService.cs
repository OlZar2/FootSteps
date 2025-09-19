using FS.Application.DTOs.AuthDTOs;
using FS.Application.DTOs.UserDTOs;
using FS.Application.Exceptions;
using FS.Application.Interfaces.QueryServices;
using FS.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FS.Persistence.QueryServices;

public class EFUserQueryService(ApplicationDbContext context) : IUserQueryService
{
    // public async Task<ProfileInfo> GetProfileInfoByIdAsync(Guid id)
    // {
    //     from u in context.Users.AsNoTracking()
    //         join fa in context.FindAnnouncements.AsNoTracking() on u.Id equals fa.CreatorId
    //         join ma in context.MissingAnnouncements.AsNoTracking() on fa.Id equals ma.CreatorId
    //         where u.Id == id
    //         select new ProfileInfo
    //         {
    //             FirstName = u.FullName.FirstName,
    //             SecondName = u.FullName.SecondName,
    //             Patronymic = u.FullName.Patronymic,
    //             Description = u.Description,
    //             UserContacts = u.Contacts.Select(UserContactData.From).ToArray(),
    //             FindAnnouncements = FinAnnounce
    //         }
    // }

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
            .FirstOrDefaultAsync()
            ?? throw new NotFoundException("User", id);
        
        return result;
    }
}
