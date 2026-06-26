using FS.Application.MissingPetLogic.DTOs;
using FS.Application.Shared.DTOs;
using FS.Core.AnimalAnnouncementBC;
using FS.Core.AnimalAnnouncementBC.Specifications;

namespace FS.Application.Interfaces.QueryServices;

public interface IMissingAnnouncementQueryService
{
    Task<MissingAnnouncementFeed[]> GetFeedAsync(
        PetAnnouncementFeedSpecification<MissingAnnouncement> spec,
        DateTime? lastDateTime = null,
        CancellationToken ct = default);
    
    Task<MissingAnnouncementPage> GetForPageByIdAsync(Guid id, CancellationToken ct);

    Task<MyAnnouncementFeed[]> GetFeedForUserAsync(
        Guid id,
        DateTime? lastDateTime = null,
        CancellationToken ct = default);

    Task<MissingAnnouncementForNotifyData> GetDataForNotifyAsync(Guid id, CancellationToken ct);

    Task<Guid[]> GetCreatorDevicesByAnnouncementIdAsync(Guid announcementId, CancellationToken ct);
}