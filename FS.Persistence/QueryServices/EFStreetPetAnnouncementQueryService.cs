using FS.Application.DTOs.Shared;
using FS.Application.DTOs.StreetPetAnnouncementDTOs;
using FS.Application.DTOs.UserDTOs;
using FS.Application.Exceptions;
using FS.Application.Interfaces.QueryServices;
using FS.Core.Entities;
using FS.Core.Specifications;
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
                MainImagePath = fa.Images[0].Path,
                PetType = fa.PetType,
                EventDate = fa.EventDate,
                Location = Coordinates.From(fa.Location),
                PlaceDescription = fa.PlaceDescription,
            })
            .AsNoTracking()
            .ToArrayAsync(ct);
    }

    public async Task<StreetPetAnnouncementPage> GetForPageByIdAsync(Guid id, CancellationToken ct)
    {
        return await (from a in context.StreetPetAnnouncements.AsNoTracking()
            join u in context.Users.Include(u=> u.AvatarImage).AsNoTracking() on a.CreatorId equals u.Id
            where a.Id == id
            select new StreetPetAnnouncementPage {
                Street = a.Street,
                House = a.House,
                ImagePaths = a.Images.Select(image => image.Path).ToArray(),
                Creator = AnnouncementCreator.From(u),
                PetType = a.PetType,
                Location = Coordinates.From(a.Location),
                EventDate = a.EventDate,
                PlaceDescription = a.PlaceDescription,
            }).SingleOrDefaultAsync(ct) ?? throw new NotFoundException("StreetPetAnnouncement", nameof(id));
    }
}