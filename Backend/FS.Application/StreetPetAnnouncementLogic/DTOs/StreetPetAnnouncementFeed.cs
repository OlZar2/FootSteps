using FS.Application.Shared.DTOs;
using FS.Core.AnimalAnnouncementBC.Enums;

namespace FS.Application.StreetPetAnnouncementLogic.DTOs;

public class StreetPetAnnouncementFeed
{
    public Guid Id { get; init; }
    
    public required string? Street { get; init; }
    public required string? House { get; init; }
    public required string? District { get; init; }
    
    public required string? MainImagePath  { get; init; }
    
    public required PetType PetType { get; init; }
    
    public required CoordinatesDto Location { get; init; }
    
    public required DateTime EventDate { get; init; }
    
    public DateTime CreatedAt { get; init; }
    
    public required string? PlaceDescription { get; init; }
}