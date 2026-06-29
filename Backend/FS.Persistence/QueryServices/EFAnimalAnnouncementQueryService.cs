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
        var cursor = DecodeCursor(query.Cursor);

        var announcements = BuildAdminListQuery();
        announcements = ApplyFilters(announcements, query);
        announcements = ApplyCursor(announcements, cursor, query);
        announcements = ApplySorting(announcements, query);

        var pageItems = await announcements
            .Take(query.PageSize + 1)
            .ToArrayAsync(ct);

        var hasNextPage = pageItems.Length > query.PageSize;
        var items = pageItems
            .Take(query.PageSize)
            .Select(ToDto)
            .ToArray();

        return new AdminAnimalAnnouncementListPage
        {
            Items = items,
            NextCursor = hasNextPage ? CreateCursor(items[^1]) : null,
        };
    }

    private IQueryable<AdminAnimalAnnouncementProjection> BuildAdminListQuery()
    {
        var reportCounts = context.AnnouncementReports
            .GroupBy(report => report.AnnouncementId)
            .Select(group => new
            {
                AnnouncementId = group.Key,
                Count = group.Count(),
            });

        return
            from announcement in context.AnimalAnnouncements.AsNoTracking()
            join reportCount in reportCounts
                on announcement.Id equals reportCount.AnnouncementId into reportCountsGroup
            from reportCount in reportCountsGroup.DefaultIfEmpty()
            select new AdminAnimalAnnouncementProjection
            {
                Id = announcement.Id,
                ImagePaths = announcement.Images
                    .OrderBy(image => image.Id)
                    .Select(image => image.FullImagePath)
                    .ToArray(),
                Description = announcement.Type == AnnouncementType.Street
                    ? ((StreetPetAnnouncement)announcement).PlaceDescription
                    : ((PetAnnouncement)announcement).Description,
                ReportsCount = reportCount == null ? 0 : reportCount.Count,
                CreatedAt = announcement.CreatedAt,
                Type = announcement.Type,
            };
    }

    private static IQueryable<AdminAnimalAnnouncementProjection> ApplyFilters(
        IQueryable<AdminAnimalAnnouncementProjection> queryable,
        AdminAnimalAnnouncementListQuery query)
    {
        if (query.CreatedAtFrom.HasValue)
            queryable = queryable.Where(x => x.CreatedAt >= query.CreatedAtFrom.Value);

        if (query.CreatedAtTo.HasValue)
            queryable = queryable.Where(x => x.CreatedAt <= query.CreatedAtTo.Value);

        if (query.ReportsCountFrom.HasValue)
            queryable = queryable.Where(x => x.ReportsCount >= query.ReportsCountFrom.Value);

        if (query.ReportsCountTo.HasValue)
            queryable = queryable.Where(x => x.ReportsCount <= query.ReportsCountTo.Value);

        return queryable;
    }

    private static IQueryable<AdminAnimalAnnouncementProjection> ApplyCursor(
        IQueryable<AdminAnimalAnnouncementProjection> queryable,
        AdminAnimalAnnouncementCursor? cursor,
        AdminAnimalAnnouncementListQuery query)
    {
        if (cursor is null)
            return queryable;

        return (query.SortBy, query.SortDirection) switch
        {
            (AdminAnimalAnnouncementSortBy.CreatedAt, SortDirection.Asc) =>
                queryable.Where(x => x.CreatedAt > cursor.CreatedAt),

            (AdminAnimalAnnouncementSortBy.CreatedAt, SortDirection.Desc) =>
                queryable.Where(x => x.CreatedAt < cursor.CreatedAt),

            (AdminAnimalAnnouncementSortBy.ReportsCount, SortDirection.Asc) =>
                queryable.Where(x =>
                    x.ReportsCount > cursor.ReportsCount
                    || x.ReportsCount == cursor.ReportsCount && x.CreatedAt > cursor.CreatedAt),

            (AdminAnimalAnnouncementSortBy.ReportsCount, SortDirection.Desc) =>
                queryable.Where(x =>
                    x.ReportsCount < cursor.ReportsCount
                    || x.ReportsCount == cursor.ReportsCount && x.CreatedAt < cursor.CreatedAt),

            _ => queryable,
        };
    }

    private static IOrderedQueryable<AdminAnimalAnnouncementProjection> ApplySorting(
        IQueryable<AdminAnimalAnnouncementProjection> queryable,
        AdminAnimalAnnouncementListQuery query)
    {
        return (query.SortBy, query.SortDirection) switch
        {
            (AdminAnimalAnnouncementSortBy.CreatedAt, SortDirection.Asc) =>
                queryable
                    .OrderBy(x => x.CreatedAt)
                    .ThenBy(x => x.ReportsCount)
                    .ThenBy(x => x.Id),

            (AdminAnimalAnnouncementSortBy.ReportsCount, SortDirection.Asc) =>
                queryable
                    .OrderBy(x => x.ReportsCount)
                    .ThenBy(x => x.CreatedAt)
                    .ThenBy(x => x.Id),

            (AdminAnimalAnnouncementSortBy.ReportsCount, SortDirection.Desc) =>
                queryable
                    .OrderByDescending(x => x.ReportsCount)
                    .ThenByDescending(x => x.CreatedAt)
                    .ThenBy(x => x.Id),

            _ => queryable
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.ReportsCount)
                .ThenBy(x => x.Id),
        };
    }

    private static AdminAnimalAnnouncementCursor? DecodeCursor(string? cursor)
    {
        AdminAnimalAnnouncementCursor.TryDecode(cursor, out var decoded);
        return decoded;
    }

    private static string CreateCursor(AdminAnimalAnnouncementListItem item)
    {
        return new AdminAnimalAnnouncementCursor(item.CreatedAt, item.ReportsCount).Encode();
    }

    private static AdminAnimalAnnouncementListItem ToDto(AdminAnimalAnnouncementProjection projection)
    {
        return new AdminAnimalAnnouncementListItem
        {
            Id = projection.Id,
            ImagePaths = projection.ImagePaths,
            Description = projection.Description,
            ReportsCount = projection.ReportsCount,
            CreatedAt = projection.CreatedAt,
            Type = projection.Type,
        };
    }

    private sealed class AdminAnimalAnnouncementProjection
    {
        public Guid Id { get; init; }

        public string[] ImagePaths { get; init; } = [];

        public string? Description { get; init; }

        public int ReportsCount { get; init; }

        public DateTime CreatedAt { get; init; }

        public AnnouncementType Type { get; init; }
    }
}
