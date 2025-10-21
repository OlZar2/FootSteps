using FS.Application.DTOs.Shared;
using FS.Application.DTOs.StreetPetAnnouncementDTOs;

namespace FS.Application.Services.StreetPetAnnouncementLogic.Interfaces;

public interface IStreetPetAnnouncementService
{
    Task CreateAsync(CreateStreetPetAnnouncementData data, CancellationToken ct);

    Task<StreetPetAnnouncementFeed[]> GetFeedAsync(DateTime lastDateTime, StreetPetAnnouncementFilter filter,
        CancellationToken ct);

    Task<StreetPetAnnouncementPage> GetPageByIdAsync(Guid id, CancellationToken ct);
}