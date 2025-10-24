using System.Text.Json.Serialization;

namespace FS.Application.Interfaces.Events;


public record EmbedRequest(
    [property: JsonPropertyName("imageId")] string ImageId,
    [property: JsonPropertyName("imageUrl")] string ImageUrl
);