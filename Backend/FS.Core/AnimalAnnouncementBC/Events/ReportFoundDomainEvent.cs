using FS.Core.Shared.Abstractions;

namespace FS.Core.AnimalAnnouncementBC.Events;

public record ReportFoundDomainEvent(Guid AnnouncementId, Guid FoundUserId) : IDomainEvent { }