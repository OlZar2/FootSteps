using FS.Application.DTOs.Shared;
using FS.Application.DTOs.UserDTOs;
using FS.Core.Entities;
using FS.Core.Enums;

namespace FS.Application.DTOs.MissingAnnouncementDTOs;

public record CreatedMissingAnnouncement
{
    public Guid Id { get; init; }
    
    public DateTime CreatedAt { get; init; }
    
    public required string FullPlace { get; init; }
    public required string District { get; init; }
    public required Coordiantes Location { get; init; }
    
    public required string[] ImagePaths  { get; init; }
    
    public required AnnouncementCreator Creator { get; init; }
    
    public required PetType PetType { get; init; }
    public required Gender Gender { get; init; }
    public string? Color { get; init; }
    public string? Breed { get; init; }
    
    public required AnnouncementType Type { get; init; }
    
    public required bool IsCompleted { get; init; }
    
    public required DateTime EventDate { get; init; }
    
    public static CreatedMissingAnnouncement From(MissingAnnouncement missingAnnouncement, User creator) => new()
    {
        Id = missingAnnouncement.Id,
        CreatedAt = missingAnnouncement.CreatedAt,
        FullPlace = missingAnnouncement.FullPlace.Value,
        District = missingAnnouncement.District.Value,
        ImagePaths = missingAnnouncement.Images.Select(i => i.Path).ToArray(),
        PetType = missingAnnouncement.PetType,
        Gender = missingAnnouncement.Gender,
        Color = missingAnnouncement.Color,
        Breed = missingAnnouncement.Breed,
        Type = missingAnnouncement.Type,
        Location = Coordiantes.From(missingAnnouncement.Location),
        IsCompleted = missingAnnouncement.IsCompleted,
        Creator = AnnouncementCreator.From(creator),
        EventDate = missingAnnouncement.EventDate
    };
}