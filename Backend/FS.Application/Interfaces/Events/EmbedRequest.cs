using System.Text.Json.Serialization;

namespace FS.Application.Interfaces.Events;

public record EmbedRequest
{
    [JsonPropertyName("imageId")] 
    public Guid ImageId { get; init; }
    
    [JsonPropertyName("imageUrl")]
    public string ImageUrl { get; init; }
}
