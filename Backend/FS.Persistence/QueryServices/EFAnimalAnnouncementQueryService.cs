using FS.Application.AnnouncementLogic.DTOs;
using FS.Application.Interfaces.Events;
using FS.Application.Interfaces.QueryServices;
using FS.Application.Shared.Exceptions;
using FS.Core.AnimalAnnouncementBC;
using FS.Core.AnimalAnnouncementBC.Enums;
using FS.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FS.Persistence.QueryServices;

public class EFAnimalAnnouncementQueryService(ApplicationDbContext context) : IAnimalAnnouncementQueryService
{
    public async Task<DeleteType?> GetDeleteTypeByIdAsync(Guid announcementId, CancellationToken ct)
    {
        var announcement = await context.AnimalAnnouncements
            .Where(aa => aa.Id == announcementId)
            .Select(aa => new { aa.DeleteType })
            .FirstOrDefaultAsync(ct);

        if (announcement is null)
            throw new NotFoundException(nameof(AnimalAnnouncement), announcementId);

        return announcement.DeleteType;
    }

    public async Task<EmbedRequest[]> GetDataForEmbeddingRequestByAnnouncementId(Guid announcementId, CancellationToken ct)
    {
        return await context.AnimalAnnouncements
            .Where(aa => aa.Id == announcementId)
            .Select(aa => aa.Images.Select(i => new EmbedRequest
            {
                ImageId = i.Id,
                ImageUrl = i.FullImagePath,
            }).ToArray())
            .FirstOrDefaultAsync(ct) ?? throw new NotFoundException($"images for announcement {announcementId} not found");
    }

    public async Task<AdminAnimalAnnouncementListPage> GetAdminListAsync(
        AdminAnimalAnnouncementListQuery query,
        CancellationToken ct)
    {
        var pageSize = query.PageSize
            ?? throw new InvalidOperationException($"{nameof(AdminAnimalAnnouncementListQuery.PageSize)} must be specified.");

        AdminAnimalAnnouncementCursor.TryDecode(query.Cursor, out var cursor);

        IQueryable<AdminAnimalAnnouncementListItem> announcements = context.AnimalAnnouncements
            .AsNoTracking()
            .Select(announcement => new AdminAnimalAnnouncementListItem
            {
                Id = announcement.Id,
                ImagePaths = announcement.Images
                    .OrderBy(image => image.Id)
                    .Select(image => image.FullImagePath)
                    .ToArray(),
                Description = announcement.Type == AnnouncementType.Street
                    ? ((StreetPetAnnouncement)announcement).PlaceDescription
                    : ((PetAnnouncement)announcement).Description,
                ReportsCount = announcement.ReportCount,
                CreatedAt = announcement.CreatedAt,
                Type = announcement.Type,
            });

        if (query.CreatedAtFrom.HasValue)
            announcements = announcements.Where(x => x.CreatedAt >= query.CreatedAtFrom.Value);

        if (query.CreatedAtTo.HasValue)
            announcements = announcements.Where(x => x.CreatedAt <= query.CreatedAtTo.Value);

        if (query.ReportsCountFrom.HasValue)
            announcements = announcements.Where(x => x.ReportsCount >= query.ReportsCountFrom.Value);

        if (query.ReportsCountTo.HasValue)
            announcements = announcements.Where(x => x.ReportsCount <= query.ReportsCountTo.Value);

        if (cursor is not null)
        {
            announcements = (query.SortBy, query.SortDirection) switch
            {
                (AdminAnimalAnnouncementSortBy.CreatedAt, SortDirection.Asc) =>
                    announcements.Where(x => x.CreatedAt > cursor.CreatedAt),

                (AdminAnimalAnnouncementSortBy.CreatedAt, SortDirection.Desc) =>
                    announcements.Where(x => x.CreatedAt < cursor.CreatedAt),

                (AdminAnimalAnnouncementSortBy.ReportsCount, SortDirection.Asc) =>
                    announcements.Where(x =>
                        x.ReportsCount > cursor.ReportsCount
                        || x.ReportsCount == cursor.ReportsCount && x.CreatedAt > cursor.CreatedAt),

                (AdminAnimalAnnouncementSortBy.ReportsCount, SortDirection.Desc) =>
                    announcements.Where(x =>
                        x.ReportsCount < cursor.ReportsCount
                        || x.ReportsCount == cursor.ReportsCount && x.CreatedAt < cursor.CreatedAt),

                _ => announcements,
            };
        }

        var sortedAnnouncements = (query.SortBy, query.SortDirection) switch
        {
            (AdminAnimalAnnouncementSortBy.CreatedAt, SortDirection.Asc) =>
                announcements
                    .OrderBy(x => x.CreatedAt)
                    .ThenBy(x => x.ReportsCount)
                    .ThenBy(x => x.Id),

            (AdminAnimalAnnouncementSortBy.ReportsCount, SortDirection.Asc) =>
                announcements
                    .OrderBy(x => x.ReportsCount)
                    .ThenBy(x => x.CreatedAt)
                    .ThenBy(x => x.Id),

            (AdminAnimalAnnouncementSortBy.ReportsCount, SortDirection.Desc) =>
                announcements
                    .OrderByDescending(x => x.ReportsCount)
                    .ThenByDescending(x => x.CreatedAt)
                    .ThenBy(x => x.Id),

            _ => announcements
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.ReportsCount)
                .ThenBy(x => x.Id),
        };

        var pageItems = await sortedAnnouncements
            .Take(pageSize + 1)
            .ToArrayAsync(ct);

        var hasNextPage = pageItems.Length > pageSize;
        var items = pageItems
            .Take(pageSize)
            .ToArray();

        return new AdminAnimalAnnouncementListPage
        {
            Items = items,
            NextCursor = hasNextPage
                ? new AdminAnimalAnnouncementCursor(items[^1].CreatedAt, items[^1].ReportsCount).Encode()
                : null,
        };
    }
}
