namespace FS.Application.DTOs.SearchDTOs;

public record SearchResultDto
{
    public required string SearchImagePath  { get; init; }
    public required SimilarAnnouncement[] Results { get; init; }
    
    public required DateTime CreatedAt { get; init; }
}