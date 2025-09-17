using FS.Application.DTOs.Shared;
using FS.Application.DTOs.StreetPetAnnouncementDTOs;
using FS.Application.DTOs.UserDTOs;
using FS.Application.Exceptions;
using FS.Application.Interfaces.QueryServices;
using FS.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FS.Persistence.QueryServices;

public class EFStreetPetAnnouncementQueryService(ApplicationDbContext context) : IStreetPetAnnouncementQueryService
{
    public async Task<CreatedStreetPetAnnouncement> GetCreatedByIdAsync(Guid id, CancellationToken ct)
    {
        return await (from a in context.StreetPetAnnouncements.AsNoTracking()
            join u in context.Users.AsNoTracking() on a.CreatorId equals u.Id
            where a.Id == id
            select new CreatedStreetPetAnnouncement {
                Id = a.Id,
                FullPlace = a.FullPlace.Value,
                District = a.District.Value,
                ImagePaths = a.Images.Select(image => image.Path).ToArray(),
                Creator = AnnouncementCreator.From(u),
                PetType = a.PetType,
                Type = a.Type,
                Location = Coordinates.From(a.Location),
                EventDate = a.EventDate,
                CreatedAt = a.CreatedAt,
            }).SingleOrDefaultAsync(ct) ?? throw new NotFoundException("StreetPetAnnouncement", nameof(id));
    }
}