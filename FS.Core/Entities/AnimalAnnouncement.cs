using FS.Core.Abstractions;
using FS.Core.Enums;
using FS.Core.ValueObjects;

namespace FS.Core.Entities;

public abstract class AnimalAnnouncement : AggregateRoot
{
    public DateTime CreatedAt { get; private set; }
    
    public string? Street { get; private set; }
    
    public string? House { get; private set; }
    
    public string? District { get; private set; }
    
    public List<Image> Images  { get; private set; }
    
    public Guid CreatorId  { get; set; }
    
    public PetType PetType { get; private set; }
    
    public AnnouncementType Type { get; private set; }
    
    public CoordinatesVO Location { get; private set; }
    
    public DateTime EventDate { get; private set; }
    
    public bool IsDeleted { get; protected set; } = false;

    protected AnimalAnnouncement(
        List<Image> images,
        Guid creatorId,
        string? district,
        string? street,
        string? house,
        PetType petType,
        CoordinatesVO location,
        DateTime createdAt,
        DateTime eventDate,
        Guid? id = null) : base(id ?? Guid.NewGuid())
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