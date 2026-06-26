using FS.Application.AuthLogic.DTOs;
using FS.Application.FindAnnouncementLogic.DTOs;
using FS.Application.Interfaces.QueryServices;
using FS.Application.Shared.DTOs;
using FS.Application.Shared.Exceptions;
using FS.Application.UserLogic.DTOs;
using FS.Core.AnimalAnnouncementBC;
using FS.Core.AnimalAnnouncementBC.Specifications;
using FS.Persistence.Context;
using FS.Persistence.Extensions;
using FS.Persistence.Projections.FindAnnouncement;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace FS.Persistence.QueryServices;

public class EFFindAnnouncementQueryService(ApplicationDbContext context) : IFindAnnouncementQueryService
{
    public async Task<FindAnnouncementFeed[]> GetFeedAsync(
        PetAnnouncementFeedSpecification<FindAnnouncement> spec,
        DateTime? lastDateTime = null,
        Point? searchCenter = null,
        int? searchRadius = null,
        CancellationToken ct = default)
    {
        lastDateTime ??= DateTime.MaxValue;
        
        IQueryable<FindAnnouncement> query = context.FindAnnouncements;
        
        foreach (var include in spec.Includes) query = query.Include(include);
        
        return await query
            .OrderByDescending(ma => ma.CreatedAt)
            .Where(spec.Criteria)
            .Where(ma => !ma.IsCompleted && !ma.IsDeleted)
            .Where(ma => ma.CreatedAt < lastDateTime)
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
        var pageProjection = await (from a in context.FindAnnouncements
                .AsNoTracking()
            join creator in context.Users on a.CreatorId equals creator.Id
            where a.Id == id
            select new FindAnnouncementPageProjection
            {
                Id = a.Id,
                Street = a.Street,
                House = a.House,
                District = a.District,
                ImagesPaths = a.Images.Select(image => image.FullImagePath).ToArray(),
                Creator = new AnnouncementCreator
                {
                    Id = creator.Id,
                    FirstName = creator.FullName.FirstName,
                    SecondName = creator.FullName.SecondName,
                    Patronymic = creator.FullName.Patronymic,
                    AvatarPath = creator.AvatarImage == null ? null : creator.AvatarImage.FullImagePath,
                    Description = creator.Description,
                    Contacts = creator.Contacts.Select(uc => new ContactData
                    {
                        ContactType = uc.Type,
                        Url = uc.Url,
                    }).ToArray()
                },
                PetType = a.PetType,
                Gender = a.Gender,
                Breed = a.Breed,
                Color = a.Color,
                Type = a.Type,
                Location = a.Location,
                EventDate = a.EventDate,
                Description = a.Description,
            }).FirstOrDefaultAsync(ct);
        
        return pageProjection is null ? throw new NotFoundException("MissingAnnouncement", nameof(id)) : ToPageDto(pageProjection);
    }
    
    public async Task<MyAnnouncementFeed[]> GetFeedForUserAsync(
        Guid id,
        DateTime? lastDateTime = null,
        CancellationToken ct = default)
    {
        lastDateTime ??= DateTime.MaxValue;
        
        return await context.FindAnnouncements
            .Where(ma => ma.CreatedAt < lastDateTime && ma.CreatorId == id)
            .OrderByDescending(ma => ma.CreatedAt)
            .Where(ma => !ma.IsDeleted)
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
    
    private static FindAnnouncementPage ToPageDto(FindAnnouncementPageProjection page)
    {
        return new FindAnnouncementPage
        {
            Id = page.Id,
            Street = page.Street,
            District = page.District,
            House = page.House,
            ImagesPaths = page.ImagesPaths,
            Creator = page.Creator,
            PetType = page.PetType,
            Gender = page.Gender,
            Breed = page.Breed,
            Color = page.Color,
            Type = page.Type,
            Location = page.Location.ToCoordinatesDto(),
            EventDate = page.EventDate,
            Description = page.Description,
        };
    }
}