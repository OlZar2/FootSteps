using FS.Application.DTOs.Shared;
using FS.Core.Entities;
using FS.Core.Stores;
using FS.Core.ValueObjects;
using FS.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace FS.Persistence.Repositories;

public class EFUserDeviceRepository(ApplicationDbContext context) : IUserDeviceRepository
{
    public async Task<UserDevice[]> GetUserDevicesForMissingAnnouncementCreateNotificationAsync(
        CoordinatesVO startPoint,
        int meterRadius,
        Guid mineId,
        CancellationToken ct)
    {
        //TODO: возможно лучше в сервисе
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        var centerPoint = geometryFactory.CreatePoint(new Coordinate(
            startPoint.Longitude,
            startPoint.Latitude));

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
            select d;
    
        return await query.ToArrayAsync(ct);
    }
}