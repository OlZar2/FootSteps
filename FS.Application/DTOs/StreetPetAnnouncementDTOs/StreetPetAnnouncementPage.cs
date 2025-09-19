using FS.Application.DTOs.Shared;
using FS.Application.DTOs.UserDTOs;
using FS.Core.Enums;

namespace FS.Application.DTOs.StreetPetAnnouncementDTOs;

public class StreetPetAnnouncementPage
{
    public required string FullPlace { get; init; }
    
    public required string[] ImagePaths  { get; init; }
    
    public required AnnouncementCreator Creator  { get; init; }
    
    public required PetType PetType { get; init; }
    
    public required Coordinates Location { get; init; }
    
    public required DateTime EventDate { get; init; }
    
    public required string? PlaceDescription { get; init; }
}