using FS.Application.DTOs.Shared;
using FS.Core.Enums;

namespace FS.Application.DTOs.StreetPetAnnouncementDTOs;

public class CreateStreetPetAnnouncementData
{
    public required string? House { get; init; }
    public required string? Street { get; init; }
    public required string? District { get; init; }
    
    public required FileData[] Images  { get; init; }
    
    public required Guid CreatorId  { get; init; }
    
    public required PetType PetType { get; init; }
    
    public required Coordinates Location { get; init; }
    
    public required DateTime EventDate { get; init; }
    
    public required string? PlaceDescription { get; init; }
}