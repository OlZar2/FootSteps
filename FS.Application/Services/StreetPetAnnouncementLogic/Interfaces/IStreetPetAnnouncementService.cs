using FS.Application.DTOs.StreetPetAnnouncementDTOs;

namespace FS.Application.Services.StreetPetAnnouncementLogic.Interfaces;

public interface IStreetPetAnnouncementService
{
    Task<CreatedStreetPetAnnouncement> CreateAsync(CreateStreetPetAnnouncementData data, CancellationToken ct);
}