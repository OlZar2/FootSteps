using FS.Core.Enums;
using FS.Core.ValueObjects;
using NetTopologySuite.Geometries;

namespace FS.Core.Entities;

public abstract class AnimalAnnouncement
{
    public Guid Id { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    
    public Place FullPlace { get; private set; }
    
    public District District { get; private set; }
    
    public List<Image> Images  { get; private set; }
    
    public Guid CreatorId  { get; set; }
    
    public PetType PetType { get; private set; }
    
    public Gender Gender { get; private set; }
    
    public string? Color { get; private set; }
    
    public string? Breed { get; private set; }
    
    public AnnouncementType Type { get; private set; }
    
    public Point Location { get; private set; }
    
    public DateTime EventDate { get; private set; }
    
    public bool IsDeleted { get; protected set; } = false;

    protected AnimalAnnouncement(
        Place fullPlace,
        List<Image> images,
        Guid creatorId,
        District district,
        PetType petType,
        Gender gender,
        string? color,
        string? breed,
        Point location,
        DateTime createdAt,
        DateTime eventDate)
    {
        FullPlace = fullPlace;
        Images = images;
        CreatorId = creatorId;
        District = district;
        PetType = petType;
        Gender = gender;
        Color = color;
        Breed = breed;
        Location = location;
        CreatedAt = createdAt;
        EventDate = eventDate;
    }
    
    // EF
    protected AnimalAnnouncement() { }
}