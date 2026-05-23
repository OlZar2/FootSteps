using FS.Application.MissingPetLogic.DTOs;

namespace FS.Application.Interfaces.QueryServices;

public interface ISpottedLocationsQueryService
{
    Task<SpottedLocationDto[]> GetSpottedLocationsByAnnouncementIdAsync(
        Guid announcementId,
        CancellationToken ct);
}