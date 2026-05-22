using FS.Application.StreetPetAnnouncementLogic.DTOs;
using FS.Core.AnimalAnnouncementBC.Specifications;

namespace FS.Application.Interfaces.QueryServices;

public interface IStreetPetAnnouncementQueryService
{
    Task<StreetPetAnnouncementFeed[]> GetFeedAsync(
        StreetPetAnnouncementFeedSpecification spec,
        DateTime? lastDateTime = null,
        CancellationToken ct = default);
    
    Task<StreetPetAnnouncementPage> GetForPageByIdAsync(Guid id, CancellationToken ct);
}