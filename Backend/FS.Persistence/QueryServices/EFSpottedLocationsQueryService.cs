using FS.Application.DTOs.MissingAnnouncementDTOs;
using FS.Application.DTOs.Shared;
using FS.Application.Interfaces.QueryServices;
using FS.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FS.Persistence.QueryServices;

public class EFSpottedLocationsQueryService(ApplicationDbContext context) : ISpottedLocationsQueryService
{
    public async Task<SpottedLocationDto[]> GetSpottedLocationsByAnnouncementIdAsync(
        Guid announcementId,
        CancellationToken ct)
    {
        var result = await (from spottedLocation in context.SpottedLocations.AsNoTracking()
            join spottedUser in context.Users.AsNoTracking() on spottedLocation.SpottedUserId equals spottedUser.Id
            where spottedLocation.MissingAnnouncementId == announcementId
            select new SpottedLocationDto(
                spottedLocation.Id,
                new SpottedUserDto(
                    spottedUser.Id,
                    spottedUser.FullName.FirstName,
                    spottedUser.FullName.SecondName),
                spottedLocation.CreatedAt,
                new CoordinatesDto
                {
                    Latitude = spottedLocation.Location.Latitude,
                    Longitude = spottedLocation.Location.Longitude
                })).ToArrayAsync(ct);

        return result;
    }
}