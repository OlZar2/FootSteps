using System.Text.Json.Serialization;
using FS.Core.AnimalAnnouncementBC.Enums;

namespace FS.Application.Interfaces.Events;

public record EmbedResponse(
    [property: JsonPropertyName("imageId")] string ImageId,
    [property: JsonPropertyName("embedding")] float[]? Embedding
);