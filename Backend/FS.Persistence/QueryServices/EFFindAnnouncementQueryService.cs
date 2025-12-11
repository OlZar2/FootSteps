using FS.Application.DTOs.FindAnnouncementDTOs;
using FS.Application.DTOs.Shared;
using FS.Application.DTOs.UserDTOs;
using FS.Application.Exceptions;
using FS.Application.Interfaces.QueryServices;
using FS.Core.AnimalAnnouncementBC;
using FS.Core.AnimalAnnouncementBC.Specifications;
using FS.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FS.Persistence.QueryServices;

public class EFFindAnnouncementQueryService(ApplicationDbContext context) : IFindAnnouncementQueryService
{
    public async Task<FindAnnouncementFeed[]> GetFeedAsync(DateTime lastDateTime, 
        PetAnnouncementFeedSpecification<FindAnnouncement> spec, CancellationToken ct)
    {
        IQueryable<FindAnnouncement> query = context.FindAnnouncements;
        
        foreach (var include in spec.Includes) query = query.Include(include);
        
        return await query
            .OrderByDescending(ma => ma.CreatedAt)
            .Where(spec.Criteria)
            .Where(ma => !ma.IsCompleted && !ma.IsDeleted)
            .Where(ma => ma.CreatedAt > lastDateTime)
            .Take(20)
            .Select(fa => new FindAnnouncementFeed
            {
                Id = fa.Id,
                CreatedAt = fa.CreatedAt,
                District = fa.District,
                Description = fa.Description,
                Gender = fa.Gender,
                MainImagePath = fa.Images.First().FullImagePath,
                PetType = fa.PetType,
                EventDate = fa.EventDate,
                Breed = fa.Breed,
            })
            .AsNoTracking()
            .ToArrayAsync(ct);
    }
    
    public async Task<FindAnnouncementPage> GetForPageByIdAsync(Guid id, CancellationToken ct)
    {
        return await (from a in context.FindAnnouncements.AsNoTracking()
            join creator in context.Users on a.CreatorId equals creator.Id
            join avatar in context.Images on creator.AvatarImageId equals avatar.Id
            where a.Id == id
            select new FindAnnouncementPage {
                Id = a.Id,
                Street = a.Street,
                House = a.House,
                District = a.District,
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
            }).SingleOrDefaultAsync(ct) ?? throw new NotFoundException(nameof(FindAnnouncement), nameof(id));
    }
    
    public async Task<MyAnnouncementFeed[]> GetFeedForUserAsync(Guid id, DateTime lastDateTime, CancellationToken ct)
    {
        return await context.FindAnnouncements
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
}