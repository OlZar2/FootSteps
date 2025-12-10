using System.Text.Json.Serialization;

namespace FS.Application.Interfaces.Events;

public record SearchResponse
(
    [property: JsonPropertyName("searchId")] string SearchId,
    [property: JsonPropertyName("embedding")] float[]? Embedding
);