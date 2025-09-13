using FS.Core.Enums;
using FS.Core.ValueObjects;
using NetTopologySuite.Geometries;

namespace FS.Core.Entities;

public class FindAnnouncement : PetAnnouncement
{
    private FindAnnouncement(
        Place fullPlace,
        List<Image> images,
        Guid creatorId,
        District district,
        PetType petType,
        Gender gender,
        string? color,
        string? breed,
        bool isCompleted,
        Point location,
        DateTime createdAt,
        DateTime eventDate)
        : base(fullPlace, images, creatorId, district, petType, gender, color, breed, isCompleted, location, createdAt, eventDate)
    {
    }

    public static FindAnnouncement Create(
        Place fullPlace,
        List<Image> images,
        Guid creatorId,
        District district,
        PetType petType,
        Gender gender,
        string? color,
        string? breed,
        Point location,
        DateTime eventDate)
    {
        var createdAt = DateTime.UtcNow;
        
        return new FindAnnouncement(
            fullPlace,
            images,
            creatorId,
            district,
            petType,
            gender,
            color,
            breed,
            false,
            location,
            createdAt,
            eventDate);
    }
    
    //EF
    private FindAnnouncement() { }
}