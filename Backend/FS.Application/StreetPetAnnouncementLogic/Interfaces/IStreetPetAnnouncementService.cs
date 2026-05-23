using FS.Application.StreetPetAnnouncementLogic.DTOs;

namespace FS.Application.StreetPetAnnouncementLogic.Interfaces;

public interface IStreetPetAnnouncementService
{
    Task CreateAsync(CreateStreetPetAnnouncementData data, CancellationToken ct);

    Task<StreetPetAnnouncementFeed[]> GetFeedAsync(
        StreetPetAnnouncementFilter filter,
        DateTime? lastDateTime = null,
        CancellationToken ct = default);

    Task<StreetPetAnnouncementPage> GetPageByIdAsync(Guid id, CancellationToken ct);

    Task UpdateSimilarAnnouncementAsync(Guid streetAnnouncementImageId, CancellationToken ct);
}