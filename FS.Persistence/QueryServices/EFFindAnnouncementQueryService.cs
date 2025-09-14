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
    public async Task<CreatedFindAnnouncement> GetCreatedFindAnnouncement(Guid id, CancellationToken ct)
    {
        return await (from ma in context.FindAnnouncements.AsNoTracking()
            join u in context.Users.AsNoTracking() on ma.CreatorId equals u.Id
            where ma.Id == id
            select new CreatedFindAnnouncement {
                Id = ma.Id,
                CreatedAt = ma.CreatedAt,
                FullPlace = ma.FullPlace.Value,
                District = ma.District.Value,
                ImagePaths = ma.Images.Select(i => i.Path).ToArray(),
                PetType = ma.PetType,
                Gender = ma.Gender,
                Color = ma.Color,
                Breed = ma.Breed,
                Type = ma.Type,
                Location = Coordinates.From(ma.Location),
                IsCompleted = ma.IsCompleted,
                Creator = AnnouncementCreator.From(u),
                EventDate = ma.EventDate,
                Description = ma.Description
            }).SingleOrDefaultAsync(ct) ?? throw new NotFoundException(nameof(FindAnnouncement), nameof(id));
    }
    
    public async Task<FindAnnouncementFeed[]> GetFeedAsync(DateTime lastDateTime, 
        PetAnnouncementFeedSpecification<FindAnnouncement> spec, CancellationToken ct)
    {
        IQueryable<FindAnnouncement> query = context.FindAnnouncements;
        
        foreach (var include in spec.Includes) query = query.Include(include);
        
        return await query
            .OrderByDescending(ma => ma.CreatedAt)
            .Where(spec.Criteria)
            .Where(ma => ma.CreatedAt > lastDateTime)
            .Take(20)
            .Select(fa => new FindAnnouncementFeed
            {
                Id = fa.Id,
                CreatedAt = fa.CreatedAt,
                District = fa.District.Value,
                Gender = fa.Gender,
                MainImagePath = fa.Images[0].Path,
                PetType = fa.PetType,
            })
            .AsNoTracking()
            .ToArrayAsync(ct);
    }
    
    public async Task<FindAnnouncementPage> GetForPageByIdAsync(Guid id, CancellationToken ct)
    {
        return await (from a in context.MissingAnnouncements.AsNoTracking()
            join u in context.Users.AsNoTracking() on a.CreatorId equals u.Id
            where a.Id == id
            select new FindAnnouncementPage {
                Id = a.Id,
                FullPlace = a.FullPlace.Value,
                District = a.District.Value,
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
}