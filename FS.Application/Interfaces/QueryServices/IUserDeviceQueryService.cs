using NetTopologySuite.Geometries;

namespace FS.Application.Interfaces.QueryServices;

public interface IUserDeviceQueryService
{
    Task<Guid[]> GetUserDevicesForMissingAnnouncementCreateNotificationAsync(
        Point centerPoint,
        int meterRadius,
        Guid mineId,
        CancellationToken ct); 
}