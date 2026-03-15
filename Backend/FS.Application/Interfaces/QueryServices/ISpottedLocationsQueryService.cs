using FS.Application.DTOs.MissingAnnouncementDTOs;

namespace FS.Application.Interfaces.QueryServices;

public interface ISpottedLocationsQueryService
{
    Task<SpottedLocationDto[]> GetSpottedLocationsByAnnouncementIdAsync(
        Guid announcementId,
        CancellationToken ct);
}