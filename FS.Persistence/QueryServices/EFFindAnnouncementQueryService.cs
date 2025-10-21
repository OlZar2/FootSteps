using FS.Application.DTOs.FindAnnouncementDTOs;
using FS.Application.DTOs.Shared;
using FS.Application.DTOs.UserDTOs;
using FS.Application.Exceptions;
using FS.Application.Interfaces.QueryServices;
using FS.Core.Entities;
using FS.Core.Specifications;
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
                MainImagePath = fa.Images[0].Path,
                PetType = fa.PetType,
                EventDate = fa.EventDate,
            })
            .AsNoTracking()
            .ToArrayAsync(ct);
    }
    
    public async Task<FindAnnouncementPage> GetForPageByIdAsync(Guid id, CancellationToken ct)
    {
        return await (from a in context.MissingAnnouncements.AsNoTracking()
            join u in context.Users.Include(u=> u.AvatarImage).AsNoTracking() on a.CreatorId equals u.Id
            where a.Id == id
            select new FindAnnouncementPage {
                Id = a.Id,
                Street = a.Street,
                House = a.House,
                District = a.District,
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
            }).SingleOrDefaultAsync(ct) ?? throw new NotFoundException("MissingAnnouncement", nameof(id));
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
            })
            .Take(20)
            .ToArrayAsync(ct);
    }
}