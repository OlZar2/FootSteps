using FS.Core.Shared.Abstractions;

namespace FS.Core.AnimalAnnouncementBC.Events;

public sealed record AnnouncementCreatedDomainEvent(
    Guid AnnouncementId,
    Dictionary<Guid, string> ImagePaths
) : IDomainEvent;