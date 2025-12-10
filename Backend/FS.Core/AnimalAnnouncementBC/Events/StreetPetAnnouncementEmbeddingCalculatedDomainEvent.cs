using FS.Core.Shared.Abstractions;

namespace FS.Core.AnimalAnnouncementBC.Events;

public sealed record StreetPetAnnouncementEmbeddingCalculatedDomainEvent(
    Guid ImageId
) : IDomainEvent;