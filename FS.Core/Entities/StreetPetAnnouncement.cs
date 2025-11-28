using FS.Core.Enums;
using FS.Core.ValueObjects;

namespace FS.Core.Entities;

public class StreetPetAnnouncement : AnimalAnnouncement
{
    private readonly List<MissingAnnouncement> _similarMissingAnnouncements = [];
    
    public string? PlaceDescription  { get; private set; }
    
    public IReadOnlyList<MissingAnnouncement> SimilarMissingAnnouncements => _similarMissingAnnouncements;

    private StreetPetAnnouncement(
        string? street,
        string? house,
        List<Image> images,
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
        List<Image> images,
        Guid creatorId,
        string? district,
        PetType petType,
        CoordinatesVO location,
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
    
    public void AddSimilarMissingAnnouncements(MissingAnnouncement[] similar)
    {
        _similarMissingAnnouncements.AddRange(similar);
    }
    
    // EF
    private StreetPetAnnouncement(){}
}