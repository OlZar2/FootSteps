using FS.Core.Enums;
using FS.Core.ValueObjects;
using NetTopologySuite.Geometries;

namespace FS.Core.Entities;

public abstract class PetAnnouncement : AnimalAnnouncement
{
    public string? Description { get; private set; }
    
    public bool IsCompleted { get; private set; }
    
    public string? Color { get; private set; }
    
    public string? Breed { get; private set; }
    
    public Gender Gender { get; private set; }
    
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
        DateTime eventDate,
        string? description) : 
        base(fullPlace, images, creatorId, district, petType, location, createdAt, eventDate)
    {
        IsCompleted = isCompleted;
        Description = description;
        Color = color;
        Gender = gender;
        Breed = breed;
    }
    
    // EF
    protected PetAnnouncement() { }
}