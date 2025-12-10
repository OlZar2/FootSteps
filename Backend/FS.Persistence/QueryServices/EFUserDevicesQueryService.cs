using FS.Application.Interfaces.QueryServices;
using FS.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace FS.Persistence.QueryServices;

public class EFUserDevicesQueryService(ApplicationDbContext context) : IUserDeviceQueryService
{
    public async Task<Guid[]> GetUserDevicesForMissingAnnouncementCreateNotificationAsync(
        Point centerPoint,
        int meterRadius,
        Guid mineId,
        CancellationToken ct)
    {
        var query =
            from u in context.Users
            where u.LastCoordinates != null &&
                  EF.Functions.IsWithinDistance(
                      u.LastCoordinates!,
                      centerPoint,
                      meterRadius,
                      false) &&
                  u.Id != mineId
            from d in u.UserDevices
            select d.Id;
    
        return await query.ToArrayAsync(ct);
    }
}