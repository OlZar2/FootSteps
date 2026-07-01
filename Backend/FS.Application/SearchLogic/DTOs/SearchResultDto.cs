namespace FS.Application.SearchLogic.DTOs;

public record SearchResultDto
{
    public required string SearchImagePath  { get; init; }
    
    public required SimilarAnnouncement[] Results { get; init; }
    
    public required DateTime CreatedAt { get; init; }
    
    public required string? ErrorCode { get; init; }
}