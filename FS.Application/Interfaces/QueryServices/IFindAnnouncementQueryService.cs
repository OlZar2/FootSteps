using FS.Application.DTOs.FindAnnouncementDTOs;
using FS.Application.DTOs.Shared;
using FS.Core.Entities;
using FS.Core.Specifications;

namespace FS.Application.Interfaces.QueryServices;

public interface IFindAnnouncementQueryService
{
    Task<FindAnnouncementFeed[]> GetFeedAsync(DateTime lastDateTime,
        PetAnnouncementFeedSpecification<FindAnnouncement> spec, CancellationToken ct);

    Task<FindAnnouncementPage> GetForPageByIdAsync(Guid id, CancellationToken ct);

    Task<MyAnnouncementFeed[]> GetFeedForUserAsync(Guid id, DateTime lastDateTime, CancellationToken ct);
}