using System.Text.Json.Serialization;
using FS.Core.Enums;

namespace FS.Application.Interfaces.Events;

public record EmbedRequest
{
    [JsonPropertyName("imageId")] 
    public string ImageId { get; init; }
    
    [JsonPropertyName("imageUrl")]
    public string ImageUrl { get; init; }
    
    [JsonPropertyName("announcementType")]
    public AnnouncementType AnnouncementType { get; init; }
}
