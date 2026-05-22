using FS.Application.FindAnnouncementLogic.DTOs;
using FS.Application.Shared.DTOs;
using FS.Core.AnimalAnnouncementBC;
using FS.Core.AnimalAnnouncementBC.Specifications;
using NetTopologySuite.Geometries;

namespace FS.Application.Interfaces.QueryServices;

public interface IFindAnnouncementQueryService
{
    Task<FindAnnouncementFeed[]> GetFeedAsync(
        PetAnnouncementFeedSpecification<FindAnnouncement> spec,
        DateTime? lastDateTime = null,
        Point? searchCenter = null,
        int? searchRadius = null,
        CancellationToken ct = default);

    Task<FindAnnouncementPage> GetForPageByIdAsync(Guid id, CancellationToken ct);

    Task<MyAnnouncementFeed[]> GetFeedForUserAsync(
        Guid id,
        DateTime? lastDateTime = null,
        CancellationToken ct = default);
}