using FS.Core.Enums;
using FS.Core.ValueObjects;
using NetTopologySuite.Geometries;

namespace FS.Core.Entities;

public class StreetPetAnnouncement : AnimalAnnouncement
{
    public string? PlaceDescription  { get; private set; }

    private StreetPetAnnouncement(
        Place fullPlace,
        List<Image> images,
        Guid creatorId,
        District district,
        PetType petType,
        Point location,
        DateTime createdAt,
        DateTime eventDate,
        string? placeDescription)
        : base(fullPlace, images, creatorId, district, petType, location, createdAt, eventDate)
    {
        PlaceDescription = placeDescription;
    }

    public static StreetPetAnnouncement Create(
        Place fullPlace,
        List<Image> images,
        Guid creatorId,
        District district,
        PetType petType,
        Point location,
        DateTime eventDate,
        string? placeDescription)
    {
        //TODO:мб вынести в ролдительский класс
        var createdAt = DateTime.UtcNow;
        
        return new StreetPetAnnouncement(
            fullPlace,
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