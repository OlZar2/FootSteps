using FS.Application.DTOs.AuthDTOs;
using FS.Application.DTOs.Shared;
using FS.Application.Exceptions;
using FS.Application.Interfaces.QueryServices;
using FS.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace FS.Persistence.QueryServices;

public class EFUserQueryService(ApplicationDbContext context) : IUserQueryService
{
    public async Task<MeInfo> GetUserMainInfoByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await context.Users
            .Include(u => u.AvatarImage)
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new MeInfo
            {
                Id = u.Id,
                FirstName = u.FullName.FirstName,
                SecondName = u.FullName.SecondName,
                Patronymic = u.FullName.Patronymic,
                AvatarPath = u.AvatarImage != null ? u.AvatarImage.Path : null,
                Contacts = u.Contacts.Select(c => new MeContactData
                {
                    ContactType = c.Type,
                    Url = c.Url,
                }).ToArray(),
                Description = u.Description,
            })
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException("User", id);
        
        return result;
    }

    public async Task<Guid[]> GetUserDevicesForMissingAnnouncementCreateNotificationAsync(
        CoordinatesDto startPoint,
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
            select u.Id;
    
        return await query.ToArrayAsync(ct);
    }
}
