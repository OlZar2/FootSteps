using FS.Application.DTOs.Shared;
using FS.Application.DTOs.UserDTOs;
using FS.Core.Enums;

namespace FS.Application.DTOs.StreetPetAnnouncementDTOs;

public class CreatedStreetPetAnnouncement
{
    public required Guid Id { get; init; }
    
    public required DateTime CreatedAt { get; init; }
    
    public required string FullPlace { get; init; }
    
    public required string District { get; init; }
    
    public required string[] ImagePaths  { get; init; }
    
    public required AnnouncementCreator Creator  { get; init; }
    
    public required PetType PetType { get; init; }
    
    public required AnnouncementType Type { get; init; }
    
    public required Coordinates Location { get; init; }
    
    public required DateTime EventDate { get; init; }
}