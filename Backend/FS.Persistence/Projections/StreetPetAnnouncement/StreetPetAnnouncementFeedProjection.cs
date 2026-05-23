using FS.Core.AnimalAnnouncementBC.Enums;
using NetTopologySuite.Geometries;

namespace FS.Persistence.Projections.StreetPetAnnouncement;

public class StreetPetAnnouncementFeedProjection
{
    public Guid Id { get; init; }
    
    public required string? Street { get; init; }
    public required string? House { get; init; }
    public required string? District { get; init; }
    
    public required string? MainImagePath  { get; init; }
    
    public required PetType PetType { get; init; }
    
    public required Point Location { get; init; }
    
    public required DateTime EventDate { get; init; }
    
    public DateTime CreatedAt { get; init; }
    
    public required string? PlaceDescription { get; init; }
}