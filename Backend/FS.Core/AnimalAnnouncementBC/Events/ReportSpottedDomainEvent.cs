using FS.Core.Shared.Abstractions;
using FS.Core.Shared.ValueObjects;

namespace FS.Core.AnimalAnnouncementBC.Events;

public record ReportSpottedDomainEvent(
    Guid MissingAnnouncementId) : IDomainEvent { }