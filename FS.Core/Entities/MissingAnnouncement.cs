using FS.Core.Enums;
using FS.Core.ValueObjects;
using NetTopologySuite.Geometries;

namespace FS.Core.Entities;

public class MissingAnnouncement : PetAnnouncement
{
    public string PetName { get; private set; }
    
    private MissingAnnouncement(
        Place fullPlace,
        List<Image> images,
        User creator,
        District district,
        PetType petType,
        Gender gender,
        string? color,
        string? breed,
        bool isCompleted,
        Point location,
        string petName,
        DateTime createdAt,
        DateTime eventDate)
        : base(fullPlace, images, creator, district, petType,  gender, color, breed, isCompleted, location, createdAt, eventDate)
    {
        PetName = petName;
    }

    public static MissingAnnouncement Create(
        Place fullPlace,
        List<Image> images,
        User creator,
        District district,
        PetType petType,
        Gender gender,
        string? color,
        string? breed,
        Point location,
        string petName,
        DateTime eventDate)
    {
        var createdAt = DateTime.UtcNow;
        
        return new MissingAnnouncement(
            fullPlace,
            images,
            creator,
            district,
            petType,
            gender,
            color,
            breed,
            false,
            location,
            petName,
            createdAt,
            eventDate);
    }
    
    // EF
    private MissingAnnouncement(){}
}