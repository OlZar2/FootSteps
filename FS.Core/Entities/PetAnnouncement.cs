using FS.Core.Enums;
using FS.Core.ValueObjects;
using NetTopologySuite.Geometries;

namespace FS.Core.Entities;

public abstract class PetAnnouncement : AnimalAnnouncement
{
    public bool IsCompleted { get; private set; }
    
    protected PetAnnouncement(
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
        DateTime eventDate) : 
        base(fullPlace, images, creatorId, district, petType, gender, color, breed, location, createdAt, eventDate)
    {
        IsCompleted = isCompleted;
    }
    
    // EF
    protected PetAnnouncement() { }
}