using FS.Core.Enums;
using NetTopologySuite.Geometries;

namespace FS.Core.Entities;

public class StreetPetAnnouncement : AnimalAnnouncement
{
    public string? PlaceDescription  { get; private set; }

    private StreetPetAnnouncement(
        string? street,
        string? house,
        List<Image> images,
        Guid creatorId,
        string? district,
        PetType petType,
        Point location,
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
        List<Image> images,
        Guid creatorId,
        string? district,
        PetType petType,
        Point location,
        DateTime eventDate,
        string? placeDescription)
    {
        //TODO:мб вынести в ролдительский класс
        var createdAt = DateTime.UtcNow;
        
        return new StreetPetAnnouncement(
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
    }
    
    // EF
    private StreetPetAnnouncement(){}
}