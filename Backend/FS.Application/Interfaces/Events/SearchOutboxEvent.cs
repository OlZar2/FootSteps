using System.Text.Json.Serialization;

namespace FS.Application.Interfaces.Events;

public class SearchOutboxEvent
{
    [property: JsonPropertyName("searchId")]
    public Guid SearchId  { get; set; }
}