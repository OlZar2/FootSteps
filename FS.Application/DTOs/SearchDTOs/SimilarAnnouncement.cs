using FS.Core.Enums;

namespace FS.Application.DTOs.SearchDTOs;

public record SimilarAnnouncement
{
    public required Guid Id { get; set; }
    
    public required string MainImagePath  { get; init; }
    
    public required string? Breed { get; init; }
    
    public required string? Street { get; init; }
    public required string? House { get; init; }
    public required string? District { get; init; }
    
    public AnnouncementType Type { get; init; }
}