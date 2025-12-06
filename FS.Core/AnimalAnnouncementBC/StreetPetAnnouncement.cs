using FS.Core.AnimalAnnouncementBC.Entities;
using FS.Core.AnimalAnnouncementBC.Enums;
using FS.Core.AnimalAnnouncementBC.Events;
using FS.Core.Shared.ValueObjects;

namespace FS.Core.AnimalAnnouncementBC;

public class StreetPetAnnouncement : AnimalAnnouncement
{
    public string? PlaceDescription  { get; private set; }

    private StreetPetAnnouncement(
        string? street,
        string? house,
        List<AnimalAnnouncementImage> images,
        Guid creatorId,
        string? district,
        PetType petType,
        CoordinatesVO location,
        DateTime createdAt,
        DateTime eventDate,
        string? placeDescription)
        : base(
            street:street,
            house:house,
            images:images,
            creatorId:creatorId,
            district:district,
            petType:petType,
            location:location,
            createdAt:createdAt,
            eventDate:eventDate)
    {
        PlaceDescription = placeDescription;
    }

    public static StreetPetAnnouncement Create(
        string? street,
        string? house,
        List<AnimalAnnouncementImage> images,
        Guid creatorId,
        string? district,
        PetType petType,
        CoordinatesVO location,
        DateTime eventDate,
        string? placeDescription)
    {
        var createdAt = DateTime.UtcNow;
        
        var created = new StreetPetAnnouncement(
            street:street,
            house:house,
            images,
            creatorId,
            district,
            petType,
            location,
            createdAt,
            eventDate,
            placeDescription);
        
        created.AddDomainEvent(new AnnouncementCreatedDomainEvent(created.Id,
            created.Images
                .ToDictionary(i => i.Id, i => i.FullImagePath)));
        
        return created;
    }
    
    protected override void OnImageEmbeddingUpdated(Guid imageId)
    {
        AddDomainEvent(new StreetPetAnnouncementEmbeddingCalculatedDomainEvent(imageId));
    }
    
    // EF
    private StreetPetAnnouncement(){}
}