using FS.Application.DTOs.Shared;
using FS.Application.DTOs.UserDTOs;
using FS.Core.Enums;

namespace FS.Application.DTOs.FindAnnouncementDTOs;

public class CreatedFindAnnouncement
{
    public required Guid Id { get; init; }
    
    public required DateTime CreatedAt { get; init; }
    
    public required string[] ImagePaths { get; init; }
    
    public required PetType PetType { get; init; }
    
    public required string? Breed { get; init; }
    
    public required string? Color { get; init; }
    
    public required Gender Gender { get; init; }
    
    public required string? Description { get; init; }
    
    public required DateTime EventDate { get; init; }
    
    public required string FullPlace { get; init; }
    
    public required Coordinates Location { get; init; }
    
    public required AnnouncementCreator Creator { get; init; }
    
    public required AnnouncementType Type { get; init; }
    
    public required bool IsCompleted { get; init; }
    
    public required string District  { get; init; }
}