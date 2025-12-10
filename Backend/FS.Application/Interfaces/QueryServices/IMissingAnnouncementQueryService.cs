using FS.Application.DTOs.MissingAnnouncementDTOs;
using FS.Application.DTOs.Shared;
using FS.Core.AnimalAnnouncementBC;
using FS.Core.AnimalAnnouncementBC.Specifications;

namespace FS.Application.Interfaces.QueryServices;

public interface IMissingAnnouncementQueryService
{
    Task<MissingAnnouncementFeed[]> GetFilteredByPageAsync(DateTime lastDateTime,
        PetAnnouncementFeedSpecification<MissingAnnouncement> spec, CancellationToken ct);
    
    Task<MissingAnnouncementPage> GetForPageByIdAsync(Guid id, CancellationToken ct);

    Task<MyAnnouncementFeed[]> GetFeedForUserAsync(Guid id, DateTime lastDateTime, CancellationToken ct);

    Task<MissingAnnouncementForNotifyData> GetDataForNotifyAsync(Guid id, CancellationToken ct);
}