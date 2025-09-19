using FS.Core.Entities;
using FS.Core.Enums;

namespace FS.Application.DTOs.FindAnnouncementDTOs;

public record FindAnnouncementFeed
{
    public required Guid Id { get; init; }
    
    public required DateTime CreatedAt { get; init; }
    
    public required string District { get; init; }
    
    public required PetType PetType { get; init; }
    
    public required string MainImagePath  { get; init; }
    
    public required Gender Gender { get; init; }
    
    public required DateTime EventDate { get; init; }

    // public static FindAnnouncementFeed From(FindAnnouncement findAnnouncement)
    // {
    //     return new FindAnnouncementFeed
    //     {
    //         Id  = findAnnouncement.Id,
    //         
    //     }
    // }
}