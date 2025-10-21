using FS.Core.Enums;

namespace FS.Application.DTOs.MissingAnnouncementDTOs;

public record MissingAnnouncementFeed
{
    public required Guid Id { get; init; }
    
    public required DateTime CreatedAt { get; init; }
    
    public required string? District { get; init; }
    
    public required PetType PetType { get; init; }
    
    public required string MainImagePath  { get; init; }
    
    public required string PetName { get; init; }
    
    public required Gender Gender { get; init; }
}