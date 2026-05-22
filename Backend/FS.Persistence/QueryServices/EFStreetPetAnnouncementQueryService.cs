using FS.Application.Interfaces.QueryServices;
using FS.Application.Shared.Exceptions;
using FS.Application.StreetPetAnnouncementLogic.DTOs;
using FS.Application.UserLogic.DTOs;
using FS.Core.AnimalAnnouncementBC;
using FS.Core.AnimalAnnouncementBC.Specifications;
using FS.Persistence.Context;
using FS.Persistence.Extensions;
using FS.Persistence.Projections.StreetPetAnnouncement;
using Microsoft.EntityFrameworkCore;

namespace FS.Persistence.QueryServices;

public class EFStreetPetAnnouncementQueryService(ApplicationDbContext context) : IStreetPetAnnouncementQueryService
{
    public async Task<StreetPetAnnouncementFeed[]> GetFeedAsync(
        StreetPetAnnouncementFeedSpecification spec,
        DateTime? lastDateTime = null, 
        CancellationToken ct = default)
    {
        lastDateTime ??= DateTime.MaxValue;
        
        IQueryable<StreetPetAnnouncement> query = context.StreetPetAnnouncements;
        
        foreach (var include in spec.Includes) query = query.Include(include);
        
        var items = await query
            .OrderByDescending(ma => ma.CreatedAt)
            .Where(spec.Criteria)
            .Where(ma => ma.CreatedAt < lastDateTime)
            .Take(20)
            .Select(fa => new StreetPetAnnouncementFeedProjection
            {
                Id = fa.Id,
                CreatedAt = fa.CreatedAt,
                District = fa.District,
                Street = fa.Street,
                House = fa.House,
                MainImagePath = fa.Images
                    .OrderBy(i => i.Id)
                    .Select(i => i.FullImagePath)
                    .FirstOrDefault(),
                PetType = fa.PetType,
                EventDate = fa.EventDate,
                Location = fa.Location,
                PlaceDescription = fa.PlaceDescription
            })
            .AsNoTracking()
            .ToArrayAsync(ct);

        return items.Select(ToFeedDto).ToArray();
    }

    public async Task<StreetPetAnnouncementPage> GetForPageByIdAsync(Guid id, CancellationToken ct)
    {
        var pageProjection = await (from a in context.StreetPetAnnouncements.AsNoTracking()
            join creator in context.Users on a.CreatorId equals creator.Id
            where a.Id == id
            select new StreetPetAnnouncementPageProjection {
                Street = a.Street,
                House = a.House,
                ImagePaths = a.Images.Select(image => image.FullImagePath).ToArray(),
                Creator = new AnnouncementCreator
                {
                    Id = creator.Id,
                    FirstName = creator.FullName.FirstName,
                    SecondName = creator.FullName.FirstName,
                    Patronymic = creator.FullName.FirstName,
                    AvatarPath = creator.AvatarImage == null ? null : creator.AvatarImage.FullImagePath,
                    Description = creator.Description,
                },
                PetType = a.PetType,
                Location = a.Location,
                EventDate = a.EventDate,
                PlaceDescription = a.PlaceDescription,
            }).SingleOrDefaultAsync(ct);
        
        return pageProjection is null ? throw new NotFoundException(nameof(StreetPetAnnouncement), id) : ToPageDto(pageProjection);
    }
    
    private static StreetPetAnnouncementFeed ToFeedDto(StreetPetAnnouncementFeedProjection feedProjection)
    {
        return new StreetPetAnnouncementFeed
        {
            Id = feedProjection.Id,
            House = feedProjection.House,
            Street = feedProjection.Street,
            District = feedProjection.District,
            MainImagePath = feedProjection.MainImagePath,
            PetType = feedProjection.PetType,
            Location = feedProjection.Location.ToCoordinatesDto(),
            EventDate = feedProjection.EventDate,
            CreatedAt = feedProjection.CreatedAt,
            PlaceDescription = feedProjection.PlaceDescription,
        };
    }
    
    private static StreetPetAnnouncementPage ToPageDto(StreetPetAnnouncementPageProjection pageProjection)
    {
        return new StreetPetAnnouncementPage
        {
            Id = pageProjection.Id,
            Street = pageProjection.Street,
            House = pageProjection.House,
            ImagePaths = pageProjection.ImagePaths,
            Creator = pageProjection.Creator,
            PetType = pageProjection.PetType,
            Location = pageProjection.Location.ToCoordinatesDto(),
            EventDate = pageProjection.EventDate,
            PlaceDescription = pageProjection.PlaceDescription,
        };
    }
}