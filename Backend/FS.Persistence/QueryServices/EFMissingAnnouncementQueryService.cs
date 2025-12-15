using FS.Application.DTOs.MissingAnnouncementDTOs;
using FS.Application.DTOs.Shared;
using FS.Application.DTOs.UserDTOs;
using FS.Application.Exceptions;
using FS.Application.Interfaces.QueryServices;
using FS.Core.AnimalAnnouncementBC;
using FS.Core.AnimalAnnouncementBC.Specifications;
using FS.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FS.Persistence.QueryServices;

public class EFMissingAnnouncementQueryService(ApplicationDbContext context) : IMissingAnnouncementQueryService
{
    public async Task<MissingAnnouncementFeed[]> GetFilteredByPageAsync(DateTime lastDateTime, 
        PetAnnouncementFeedSpecification<MissingAnnouncement> spec, CancellationToken ct)
    {
        IQueryable<MissingAnnouncement> query = context.MissingAnnouncements;
        
        foreach (var include in spec.Includes) query = query.Include(include);
        
        return await query
            .OrderByDescending(ma => ma.CreatedAt)
            .Where(spec.Criteria)
            .Where(ma => ma.CreatedAt > lastDateTime)
            .Take(20)
            .Select(a => new MissingAnnouncementFeed
            {
                Id = a.Id,
                PetName = a.PetName,
                CreatedAt = a.CreatedAt,
                District = a.District,
                PetType = a.PetType,
                Gender = a.Gender,
                MainImagePath = a.Images.First().FullImagePath,
                Description = a.Description,
                Breed = a.Breed,
            })
            .ToArrayAsync(ct);
    }
    
    public async Task<MissingAnnouncementPage> GetForPageByIdAsync(Guid id, CancellationToken ct)
    {
        return await (from a in context.MissingAnnouncements.AsNoTracking()
            join creator in context.Users on a.CreatorId equals creator.Id
            join avatar in context.Images on creator.AvatarImageId equals avatar.Id
            where a.Id == id
            select new MissingAnnouncementPage {
                Id = a.Id,
                Street = a.Street,
                District = a.District,
                House = a.House,
                ImagesPaths = a.Images.Select(image => image.FullImagePath).ToArray(),
                Creator = AnnouncementCreator.FromUserAndAvatar(creator, avatar),
                PetType = a.PetType,
                Gender = a.Gender,
                Breed = a.Breed,
                Color = a.Color,
                Type = a.Type,
                Location = new CoordinatesDto()
                {
                    Latitude = a.Location.Latitude,
                    Longitude = a.Location.Longitude
                },
                EventDate = a.EventDate,
                Description = a.Description,
                PetName = a.PetName,
                SimilarAnnouncements = (
                    from sa in context.SimilarAnnouncements
                    join ann in context.StreetPetAnnouncements on sa.StreetPetAnnouncementId equals ann.Id
                    where sa.MissingAnnouncementId == a.Id
                    select new SimilarMapAnnouncement {
                        Id = ann.Id,
                        Coordinates = new CoordinatesDto {
                            Latitude = ann.Location.Latitude,
                            Longitude = ann.Location.Longitude
                        },
                        CreatedAt = ann.CreatedAt
                    }
                ).ToArray()
            }).SingleOrDefaultAsync(ct) ?? throw new NotFoundException("MissingAnnouncement", nameof(id));
    }

    public async Task<MyAnnouncementFeed[]> GetFeedForUserAsync(Guid id, DateTime lastDateTime, CancellationToken ct)
    {
        return await context.MissingAnnouncements
            .Where(ma => ma.CreatedAt > lastDateTime && ma.CreatorId == id)
            .Select(ma => new MyAnnouncementFeed
            {   
                Id = ma.Id,
                CreatedAt = ma.CreatedAt,
                Description = ma.Description,
                District = ma.District,
                Street = ma.Street,
                Breed = ma.Breed,
                MainImagePath = ma.Images.First().FullImagePath,
            })
            .Take(20)
            .ToArrayAsync(ct);
    }

    public async Task<MissingAnnouncementForNotifyData> GetDataForNotifyAsync(Guid id, CancellationToken ct)
    {
        return await context.MissingAnnouncements
            .Where(ma => ma.Id == id)
            .Select(ma => new MissingAnnouncementForNotifyData
            {
                Coordinates = CoordinatesDto.From(ma.Location),
                CreatorId = ma.CreatorId,
            })
            .FirstOrDefaultAsync(ct) ?? throw new NotFoundException(nameof(MissingAnnouncement), nameof(id));;
    }
}