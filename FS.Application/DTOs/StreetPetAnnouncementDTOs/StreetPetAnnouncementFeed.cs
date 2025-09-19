using FS.Application.DTOs.Shared;
using FS.Core.Enums;

namespace FS.Application.DTOs.StreetPetAnnouncementDTOs;

public class StreetPetAnnouncementFeed
{
    public Guid Id { get; init; }
    
    public required string FullPlace { get; init; }
    
    public required string District { get; init; }
    
    public required string MainImagePath  { get; init; }
    
    public required PetType PetType { get; init; }
    
    public required Coordinates Location { get; init; }
    
    public required DateTime EventDate { get; init; }
    
    public DateTime CreatedAt { get; init; }
    
    public required string? PlaceDescription { get; init; }
}