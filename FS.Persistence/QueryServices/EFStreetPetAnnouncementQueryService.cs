using FS.Application.DTOs.Shared;
using FS.Application.DTOs.StreetPetAnnouncementDTOs;
using FS.Application.DTOs.UserDTOs;
using FS.Application.Exceptions;
using FS.Application.Interfaces.QueryServices;
using FS.Core.AnimalAnnouncementBC;
using FS.Core.AnimalAnnouncementBC.Specifications;
using FS.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FS.Persistence.QueryServices;

public class EFStreetPetAnnouncementQueryService(ApplicationDbContext context) : IStreetPetAnnouncementQueryService
{
    public async Task<StreetPetAnnouncementFeed[]> GetFeedAsync(DateTime lastDateTime, 
        StreetPetAnnouncementFeedSpecification spec, CancellationToken ct)
    {
        IQueryable<StreetPetAnnouncement> query = context.StreetPetAnnouncements;
        
        foreach (var include in spec.Includes) query = query.Include(include);
        
        return await query
            .OrderByDescending(ma => ma.CreatedAt)
            .Where(spec.Criteria)
            .Where(ma => ma.CreatedAt > lastDateTime)
            .Take(20)
            .Select(fa => new StreetPetAnnouncementFeed
            {
                Id = fa.Id,
                CreatedAt = fa.CreatedAt,
                District = fa.District,
                Street = fa.Street,
                House = fa.House,
                MainImagePath = fa.Images.First().FullImagePath,
                PetType = fa.PetType,
                EventDate = fa.EventDate,
                Location = new CoordinatesDto
                {
                    Latitude = fa.Location.Latitude,
                    Longitude = fa.Location.Longitude
                },
                PlaceDescription = fa.PlaceDescription,
            })
            .AsNoTracking()
            .ToArrayAsync(ct);
    }

    public async Task<StreetPetAnnouncementPage> GetForPageByIdAsync(Guid id, CancellationToken ct)
    {
        return await (from a in context.StreetPetAnnouncements.AsNoTracking()
            join creator in context.Users on a.CreatorId equals creator.Id
            join avatar in context.AnimalAnnouncementImages on creator.AvatarImageId equals avatar.Id 
            where a.Id == id
            select new StreetPetAnnouncementPage {
                Street = a.Street,
                House = a.House,
                ImagePaths = a.Images.Select(image => image.FullImagePath).ToArray(),
                Creator = AnnouncementCreator.FromUserAndAvatar(creator, avatar),
                PetType = a.PetType,
                Location = new CoordinatesDto
                {
                    Latitude = a.Location.Latitude,
                    Longitude = a.Location.Longitude
                },
                EventDate = a.EventDate,
                PlaceDescription = a.PlaceDescription,
            }).SingleOrDefaultAsync(ct) ?? throw new NotFoundException("StreetPetAnnouncement", nameof(id));
    }
}