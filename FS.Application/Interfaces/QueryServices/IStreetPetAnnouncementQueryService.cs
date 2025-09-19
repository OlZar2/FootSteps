using FS.Application.DTOs.StreetPetAnnouncementDTOs;
using FS.Core.Specifications;

namespace FS.Application.Interfaces.QueryServices;

public interface IStreetPetAnnouncementQueryService
{
    Task<CreatedStreetPetAnnouncement> GetCreatedByIdAsync(Guid id, CancellationToken ct);
    
    Task<StreetPetAnnouncementFeed[]> GetFeedAsync(DateTime lastDateTime,
        StreetPetAnnouncementFeedSpecification spec, CancellationToken ct);
    
    Task<StreetPetAnnouncementPage> GetForPageByIdAsync(Guid id, CancellationToken ct);
}