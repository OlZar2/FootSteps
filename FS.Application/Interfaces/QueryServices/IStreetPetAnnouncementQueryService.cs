using FS.Application.DTOs.StreetPetAnnouncementDTOs;

namespace FS.Application.Interfaces.QueryServices;

public interface IStreetPetAnnouncementQueryService
{
    Task<CreatedStreetPetAnnouncement> GetCreatedByIdAsync(Guid id, CancellationToken ct);
}