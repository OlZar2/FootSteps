using FS.Core.Enums;

namespace FS.Application.DTOs.MissingAnnouncementDTOs;

public record MissingAnnouncementFeed
{
    public Guid Id { get; init; }
    
    public DateTime CreatedAt { get; init; }
    
    public required string District { get; init; }
    
    public PetType PetType { get; init; }
    
    public required string MainImagePath  { get; init; }
    
    public required string PetName { get; init; }
    
    public Gender Gender { get; init; }
}