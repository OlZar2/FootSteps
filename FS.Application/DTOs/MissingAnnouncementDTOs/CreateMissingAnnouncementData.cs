using FS.Application.DTOs.Shared;
using FS.Core.Enums;
using NetTopologySuite.Geometries;

namespace FS.Application.DTOs.MissingAnnouncementDTOs;

public class CreateMissingAnnouncementData
{
    public required string FullPlace { get; init; }
    public required string District { get; init; }
    public required Coordiantes Location { get; init; }
    
    public required FileData[] Images  { get; init; }
    
    public required Guid CreatorId { get; init; }
    
    public required PetType PetType { get; init; }
    public required Gender Gender { get; init; }
    public string? Color { get; init; }
    public string? Breed { get; init; }
    public required string PetName { get; init; }
    
    public required DateTime EventDate { get; init; }
}