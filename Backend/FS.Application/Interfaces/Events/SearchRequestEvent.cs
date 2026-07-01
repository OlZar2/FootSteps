using System.Text.Json.Serialization;

namespace FS.Application.Interfaces.Events;

public record SearchRequestEvent(
    [property: JsonPropertyName("searchId")] string SearchId,
    [property: JsonPropertyName("imageUrl")] string ImageUrl
);