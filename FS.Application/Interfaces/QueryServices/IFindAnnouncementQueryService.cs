using FS.Application.DTOs.FindAnnouncementDTOs;
using FS.Core.Entities;
using FS.Core.Specifications;

namespace FS.Application.Interfaces.QueryServices;

public interface IFindAnnouncementQueryService
{
    Task<CreatedFindAnnouncement> GetCreatedFindAnnouncement(Guid id, CancellationToken ct);

    Task<FindAnnouncementFeed[]> GetFeedAsync(DateTime lastDateTime,
        PetAnnouncementFeedSpecification<FindAnnouncement> spec, CancellationToken ct);

    Task<FindAnnouncementPage> GetForPageByIdAsync(Guid id, CancellationToken ct);
}