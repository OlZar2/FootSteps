using FS.Core.Abstractions;
using FS.Core.Enums;
using FS.Core.ValueObjects;

namespace FS.Core.Events;

public sealed record MissingAnnouncementCreatedDomainEvent(
    Guid AnnouncementId,
    CoordinatesVO CoordinatesVo,
    Guid CreatorId
) : IDomainEvent;