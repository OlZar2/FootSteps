using System.Text.Json.Serialization;
using FS.Core.Shared.Abstractions;

namespace FS.Core.AnimalAnnouncementBC.Events;

public sealed record MissingAnnouncementCreatedDomainEvent(
    [property: JsonPropertyName("announcementId")] 
    Guid AnnouncementId
) : IDomainEvent;