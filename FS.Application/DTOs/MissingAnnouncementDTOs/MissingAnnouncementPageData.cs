using FS.Application.DTOs.Shared;
using FS.Application.DTOs.UserDTOs;
using FS.Core.Entities;
using FS.Core.Enums;

namespace FS.Application.DTOs.MissingAnnouncementDTOs;

public record MissingAnnouncementPageData
{
    public required Guid Id { get; init; }
    
    public required string FullPlace { get; init; }
    
    public required string District { get; init; }
    
    public required string[] ImagesPaths  { get; init; }
    
    public required AnnouncementCreator Creator  { get; init; }
    
    public required PetType PetType { get; init; }
    
    public required Gender Gender { get; init; }
    
    public required string? Color { get; init; }
    
    public required string? Breed { get; init; }
    
    public required AnnouncementType Type { get; init; }
    
    public required Coordiantes Location { get; init; }
    
    public required DateTime EventDate { get;init; }

    public static MissingAnnouncementPageData From(MissingAnnouncement missingAnnouncement)
    {
        return new MissingAnnouncementPageData
        {
            Id = missingAnnouncement.Id,
            FullPlace = missingAnnouncement.FullPlace.Value,
            District = missingAnnouncement.District.Value,
            ImagesPaths = missingAnnouncement.Images.Select(i => i.Path).ToArray(),
            Creator = AnnouncementCreator.From(missingAnnouncement.Creator),
            PetType = missingAnnouncement.PetType,
            Gender = missingAnnouncement.Gender,
            Color = missingAnnouncement.Color,
            Breed = missingAnnouncement.Breed,
            Type = missingAnnouncement.Type,
            Location = Coordiantes.From(missingAnnouncement.Location),
            EventDate = missingAnnouncement.EventDate
        };
    }
}