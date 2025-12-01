using FS.Application.DTOs.AuthDTOs;
using FS.Application.DTOs.Shared;

namespace FS.Application.Interfaces.QueryServices;

public interface IUserQueryService
{
    Task<MeInfo> GetUserMainInfoByIdAsync(Guid id, CancellationToken ct);

    Task<Guid[]> GetUserDevicesForMissingAnnouncementCreateNotificationAsync(
        CoordinatesDto startPoint,
        int meterRadius,
        Guid mineId,
        CancellationToken ct);
}