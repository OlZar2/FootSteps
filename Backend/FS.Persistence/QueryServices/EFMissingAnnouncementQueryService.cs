using FS.Application.AuthLogic.DTOs;
using FS.Application.Interfaces.QueryServices;
using FS.Application.MissingPetLogic.DTOs;
using FS.Application.Shared.DTOs;
using FS.Application.Shared.Exceptions;
using FS.Application.UserLogic.DTOs;
using FS.Core.AnimalAnnouncementBC;
using FS.Core.AnimalAnnouncementBC.Enums;
using FS.Core.AnimalAnnouncementBC.Specifications;
using FS.Persistence.Context;
using FS.Persistence.Extensions;
using FS.Persistence.Projections.MissingAnnouncement;
using FS.Persistence.Projections.Shared;
using Microsoft.EntityFrameworkCore;

namespace FS.Persistence.QueryServices;

public class EFMissingAnnouncementQueryService(ApplicationDbContext context) : IMissingAnnouncementQueryService
{
    public async Task<MissingAnnouncementFeed[]> GetFeedAsync(
        PetAnnouncementFeedSpecification<MissingAnnouncement> spec,
        DateTime? lastDateTime = null,
        CancellationToken ct = default)
    {
        lastDateTime ??= DateTime.MaxValue;
        
        IQueryable<MissingAnnouncement> query = context.MissingAnnouncements;
        
        foreach (var include in spec.Includes) query = query.Include(include);
        
        return await query
            .OrderByDescending(ma => ma.CreatedAt)
            .Where(spec.Criteria)
            .Where(ma => ma.CreatedAt < lastDateTime)
            .Where(ma => !ma.IsCompleted && ma.DeleteType == null)
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
        var page = await (
            from a in context.MissingAnnouncements.AsNoTracking()
            join creator in context.Users on a.CreatorId equals creator.Id
            where a.Id == id
            select new MissingAnnouncementPageProjection
            {
                Id = a.Id,
                Street = a.Street,
                District = a.District,
                House = a.House,
                ImagesPaths = a.Images.Select(image => image.FullImagePath).ToArray(),
                Creator = new AnnouncementCreator
                {
                    Id = creator.Id,
                    FirstName = creator.FullName.FirstName,
                    SecondName = creator.FullName.SecondName,
                    Patronymic = creator.FullName.Patronymic,
                    AvatarPath = creator.AvatarImage == null 
                        ? null 
                        : creator.AvatarImage.FullImagePath,
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
                PetName = a.PetName,
                SimilarAnnouncements = (
                    from sa in context.SimilarAnnouncements
                    join ann in context.StreetPetAnnouncements 
                        on sa.StreetPetAnnouncementId equals ann.Id
                    where sa.MissingAnnouncementId == a.Id && ann.DeleteType == null
                    select new SimilarMapAnnouncementProjection
                    {
                        Id = ann.Id,
                        Location = ann.Location,
                        CreatedAt = ann.CreatedAt
                    }
                ).ToArray()
            }
        ).SingleOrDefaultAsync(ct);

        return page is null ? throw new NotFoundException("MissingAnnouncement", nameof(id)) : ToPageDto(page);
    }

    public async Task<MyAnnouncementFeed[]> GetFeedForUserAsync(
        Guid id,
        DateTime? lastDateTime = null,
        CancellationToken ct = default)
    {
        lastDateTime ??= DateTime.MaxValue;
        
        return await context.MissingAnnouncements
            .Where(ma => ma.CreatedAt < lastDateTime && ma.CreatorId == id)
            .Where(ma => ma.DeleteType != DeleteType.UserCancel)
            .OrderByDescending(ma => ma.CreatedAt)
            .Select(ma => new MyAnnouncementFeed
            {   
                Id = ma.Id,
                CreatedAt = ma.CreatedAt,
                Description = ma.Description,
                District = ma.District,
                Street = ma.Street,
                Breed = ma.Breed,
                IsDeletedByAdmin = ma.DeleteType == DeleteType.AdminHide,
                MainImagePath = ma.Images.First().FullImagePath,
            })
            .Take(20)
            .ToArrayAsync(ct);
    }

    public async Task<MissingAnnouncementForNotifyData> GetDataForNotifyAsync(
        Guid id, 
        CancellationToken ct)
    {
        var data = await context.MissingAnnouncements
            .AsNoTracking()
            .Where(ma => ma.Id == id)
            .Select(ma => new
            {
                ma.Location,
                ma.CreatorId
            })
            .FirstOrDefaultAsync(ct);

        if (data is null)
            throw new NotFoundException(nameof(MissingAnnouncement), nameof(id));

        return new MissingAnnouncementForNotifyData
        {
            Coordinates = data.Location.ToCoordinatesDto(),
            CreatorId = data.CreatorId
        };
    }

    public async Task<Guid[]> GetCreatorDevicesByAnnouncementIdAsync(Guid announcementId, CancellationToken ct)
    {
        return await (
            from a in context.MissingAnnouncements.AsNoTracking()
            join creator in context.Users on a.CreatorId equals creator.Id
            from device in creator.UserDevices
            where a.Id == announcementId && device.IsActive
            select device.Id
        ).ToArrayAsync(ct);
    }
    
    private static MissingAnnouncementPage ToPageDto(MissingAnnouncementPageProjection page)
    {
        return new MissingAnnouncementPage
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
            PetName = page.PetName,
            SimilarAnnouncements = page.SimilarAnnouncements
                .Select(ToSimilarMapAnnouncement)
                .ToArray()
        };
    }

    private static SimilarMapAnnouncement ToSimilarMapAnnouncement(
        SimilarMapAnnouncementProjection source)
    {
        return new SimilarMapAnnouncement
        {
            Id = source.Id,
            Coordinates = source.Location.ToCoordinatesDto(),
            CreatedAt = source.CreatedAt
        };
    }
}
