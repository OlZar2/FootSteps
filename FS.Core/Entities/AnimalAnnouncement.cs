using FS.Core.Enums;
using NetTopologySuite.Geometries;

namespace FS.Core.Entities;

public abstract class AnimalAnnouncement
{
    public Guid Id { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    
    public string? Street { get; private set; }
    
    public string? House { get; private set; }
    
    public string? District { get; private set; }
    
    public List<Image> Images  { get; private set; }
    
    public Guid CreatorId  { get; set; }
    
    public PetType PetType { get; private set; }
    
    public AnnouncementType Type { get; private set; }
    
    public Point Location { get; private set; }
    
    public DateTime EventDate { get; private set; }
    
    public bool IsDeleted { get; protected set; } = false;

    protected AnimalAnnouncement(
        List<Image> images,
        Guid creatorId,
        string? district,
        string? street,
        string? house,
        PetType petType,
        Point location,
        DateTime createdAt,
        DateTime eventDate)
    {
        Street = street;
        House = house;
        Images = images;
        CreatorId = creatorId;
        District = district;
        PetType = petType;
        Location = location;
        CreatedAt = createdAt;
        EventDate = eventDate;
    }
    
    // EF
    protected AnimalAnnouncement() { }
}