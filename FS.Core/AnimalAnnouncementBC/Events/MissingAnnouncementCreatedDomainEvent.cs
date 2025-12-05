using FS.Core.Shared.Abstractions;
using FS.Core.Shared.ValueObjects;

namespace FS.Core.AnimalAnnouncementBC.Events;

public sealed record MissingAnnouncementCreatedDomainEvent(
    Guid AnnouncementId,
    CoordinatesVO CoordinatesVo,
    Guid CreatorId
) : IDomainEvent;