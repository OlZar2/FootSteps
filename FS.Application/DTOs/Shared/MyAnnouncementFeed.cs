namespace FS.Application.DTOs.Shared;

public class MyAnnouncementFeed
{
    public required Guid Id { get; init; }
    
    public required string? Breed { get; init; }
    
    public required string? Description { get; init; }
    
    public required string? District { get; init; }
    
    public required string? Street { get; init; }
    
    public DateTime CreatedAt { get; init; }
}