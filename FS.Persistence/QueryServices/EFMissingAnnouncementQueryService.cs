using FS.Application.DTOs.MissingAnnouncementDTOs;
using FS.Application.DTOs.Shared;
using FS.Application.DTOs.UserDTOs;
using FS.Application.Exceptions;
using FS.Application.Interfaces.QueryServices;
using FS.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FS.Persistence.QueryServices;

public class EFMissingAnnouncementQueryService(ApplicationDbContext context) : IMissingAnnouncementQueryService
{
    public async Task<MissingAnnouncementPageData> GetForPageByIdAsync(Guid id, CancellationToken ct)
    {
        return await (from a in context.MissingAnnouncements.AsNoTracking()
            join u in context.Users.AsNoTracking() on a.CreatorId equals u.Id
            where a.Id == id
            select new MissingAnnouncementPageData {
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
                Location = Coordiantes.From(a.Location),
                EventDate = a.EventDate
            }).SingleOrDefaultAsync(ct) ?? throw new NotFoundException("MissingAnnouncement", nameof(id));
    }

    public async Task<CreatedMissingAnnouncement> GetCreatedByIdAsync(Guid id, CancellationToken ct)
    {
        return await (from ma in context.MissingAnnouncements.AsNoTracking()
            join u in context.Users.AsNoTracking() on ma.CreatorId equals u.Id
            where ma.Id == id
            select new CreatedMissingAnnouncement {
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
                Location = Coordiantes.From(ma.Location),
                IsCompleted = ma.IsCompleted,
                Creator = AnnouncementCreator.From(u),
                EventDate = ma.EventDate
            }).SingleOrDefaultAsync(ct) ?? throw new NotFoundException("MissingAnnouncement", nameof(id));
    }
}