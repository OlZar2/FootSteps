using FS.Core.Entities;
using Pgvector;

namespace FS.Core.Stores;

public interface IStreetPetAnnouncementRepository
{
    Task CreateAsync(StreetPetAnnouncement missingAnnouncement, CancellationToken ct);

    Task<StreetPetAnnouncement[]> GetSimilarStreetPets(Vector vector, CancellationToken ct);
}