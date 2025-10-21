using FS.Application.DTOs.Shared;
using FS.Core.Enums;

namespace FS.Application.DTOs.FindAnnouncementDTOs;

public record CreateFindAnnouncementData
{
    public required FileData[] Images { get; init; }
    
    public required PetType PetType { get; init; }
    
    public string? Breed { get; init; }
    
    public string? Color { get; init; }
    
    public Gender Gender { get; init; }
    
    public string? Description { get; init; }
    
    public required DateTime EventDate { get; init; }
    
    public required Coordinates Location { get; init; }
    
    public required Guid CreatorId { get; init; }
    
    public required string? District { get; init; }
    public required string? House { get; init; }
    public required string? Street { get; init; }
}