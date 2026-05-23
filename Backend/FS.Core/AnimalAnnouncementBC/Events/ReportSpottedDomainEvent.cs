using FS.Core.Shared.Abstractions;

namespace FS.Core.AnimalAnnouncementBC.Events;

public record ReportSpottedDomainEvent(
    Guid MissingAnnouncementId) : IDomainEvent { }