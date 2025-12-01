using FS.Core.Entities;
using FS.Core.ValueObjects;

namespace FS.Core.Stores;

public interface IUserDeviceRepository
{
    Task<UserDevice[]> GetUserDevicesForMissingAnnouncementCreateNotificationAsync(
        CoordinatesVO startPoint,
        int meterRadius,
        Guid mineId,
        CancellationToken ct);
}