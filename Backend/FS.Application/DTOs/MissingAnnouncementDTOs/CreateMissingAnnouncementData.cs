using FS.Application.DTOs.Shared;
using FS.Core.AnimalAnnouncementBC.Enums;

namespace FS.Application.DTOs.MissingAnnouncementDTOs;

public record CreateMissingAnnouncementData
{
    public required string? Street { get; init; }
    public required string? House { get; init; }
    public required string? District { get; init; }
    
    public required CoordinatesDto Location { get; init; }
    
    public required Guid[] ImageIds  { get; init; }
    
    public required Guid CreatorId { get; init; }
    
    public required PetType PetType { get; init; }
    public required Gender Gender { get; init; }
    public string? Color { get; init; }
    public string? Breed { get; init; }
    public required string PetName { get; init; }
    
    public required DateTime EventDate { get; init; }
    public string? Description { get; init; }
}