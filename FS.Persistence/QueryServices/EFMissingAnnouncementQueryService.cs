using FS.Application.DTOs.MissingAnnouncementDTOs;
using FS.Application.DTOs.Shared;
using FS.Application.DTOs.UserDTOs;
using FS.Application.Exceptions;
using FS.Application.Interfaces.QueryServices;
using FS.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FS.Persistence.QueryServices;

public class EFMissingAnnouncementQueryService(ApplicationDbContext context) : IMissingAnnouncementQueryService
{
    public async Task<MissingAnnouncementPage> GetForPageByIdAsync(Guid id, CancellationToken ct)
    {
        return await (from a in context.MissingAnnouncements.AsNoTracking()
            join u in context.Users.Include(u=> u.AvatarImage).AsNoTracking() on a.CreatorId equals u.Id
            where a.Id == id
            select new MissingAnnouncementPage {
                Id = a.Id,
                Street = a.Street,
                District = a.District,
                House = a.House,
                ImagesPaths = a.Images.Select(image => image.Path).ToArray(),
                Creator = AnnouncementCreator.From(u),
                PetType = a.PetType,
                Gender = a.Gender,
                Breed = a.Breed,
                Color = a.Color,
                Type = a.Type,
                Location = Coordinates.From(a.Location),
                EventDate = a.EventDate,
                Description = a.Description,
                PetName = a.PetName,
            }).SingleOrDefaultAsync(ct) ?? throw new NotFoundException("MissingAnnouncement", nameof(id));
    }
}