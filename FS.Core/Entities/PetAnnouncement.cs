using FS.Core.Enums;
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
        string? street,
        string? house,
        List<Image> images,
        Guid creatorId,
        string? district,
        PetType petType,
        Gender gender,
        string? color,
        string? breed,
        bool isCompleted,
        Point location,
        DateTime createdAt,
        DateTime eventDate,
        string? description) : 
        base(
            street: street,
            house: house,
            images: images,
            creatorId: creatorId,
            district:district,
            petType:petType,
            location:location,
            createdAt:createdAt,
            eventDate:eventDate
        )
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