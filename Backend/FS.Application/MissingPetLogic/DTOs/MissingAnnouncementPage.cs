using FS.Application.Shared.DTOs;
using FS.Application.UserLogic.DTOs;
using FS.Core.AnimalAnnouncementBC.Enums;

namespace FS.Application.MissingPetLogic.DTOs;

public record MissingAnnouncementPage
{
    public required Guid Id { get; init; }
    
    public required string? Street { get; init; }
    public required string? House { get; init; }
    public required string? District { get; init; }
    
    public required string[] ImagesPaths  { get; init; }
    
    public required AnnouncementCreator Creator  { get; init; }
    
    public required PetType PetType { get; init; }
    
    public required Gender Gender { get; init; }
    
    public required string? Color { get; init; }
    
    public required string? Breed { get; init; }
    
    public required AnnouncementType Type { get; init; }
    
    public required CoordinatesDto Location { get; init; }
    
    public required DateTime EventDate { get;init; }
    
    public required string? Description { get; init; }
    
    public required string PetName { get; init; }

    public SimilarMapAnnouncement[] SimilarAnnouncements { get; init; } = [];
}