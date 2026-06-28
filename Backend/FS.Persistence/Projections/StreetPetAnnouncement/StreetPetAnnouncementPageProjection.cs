using FS.Application.UserLogic.DTOs;
using FS.Core.AnimalAnnouncementBC.Enums;
using NetTopologySuite.Geometries;

namespace FS.Persistence.Projections.StreetPetAnnouncement;

public class StreetPetAnnouncementPageProjection
{
    public Guid Id { get; set; }
    
    public required string? Street { get; init; }
    public required string? House { get; init; }
    
    public required string[] ImagePaths  { get; init; }
    
    public required AnnouncementCreator Creator  { get; init; }
    
    public required PetType PetType { get; init; }
    
    public required Point Location { get; init; }
    
    public required DateTime EventDate { get; init; }
    
    public required string? PlaceDescription { get; init; }

    public DeleteType? DeleteType { get; init; }
}
