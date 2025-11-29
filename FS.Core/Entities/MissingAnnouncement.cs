using FS.Contracts.Error;
using FS.Core.Enums;
using FS.Core.Events;
using FS.Core.Exceptions;
using FS.Core.Policies.AnnouncementPolicies;
using FS.Core.ValueObjects;

namespace FS.Core.Entities;

public class MissingAnnouncement : PetAnnouncement
{
    private readonly List<StreetPetAnnouncement> _similarStreetAnnouncements = [];
    
    public string PetName { get; private set; }
    
    public IReadOnlyList<StreetPetAnnouncement> SimilarStreetAnnouncements => _similarStreetAnnouncements;
    
    public MissingAnnouncementDeleteReason DeleteReason { get; private set; }
    
    private MissingAnnouncement(
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
        CoordinatesVO location,
        string petName,
        DateTime createdAt,
        DateTime eventDate,
        string? description)
        : base(
            street: street,
            house: house,
            images,
            creatorId,
            district,
            petType,
            gender,
            color,
            breed,
            isCompleted,
            location,
            createdAt,
            eventDate,
            description)
    {
        PetName = petName;
    }

    public static MissingAnnouncement Create(
        string? street,
        string? house,
        List<Image> images,
        Guid creatorId,
        string? district,
        PetType petType,
        Gender gender,
        string? color,
        string? breed,
        CoordinatesVO location,
        string petName,
        DateTime eventDate,
        string? description)
    {
        var createdAt = DateTime.UtcNow;
        
        var created = new MissingAnnouncement(
            street: street,
            house: house,
            images,
            creatorId,
            district,
            petType,
            gender,
            color,
            breed,
            false,
            location,
            petName,
            createdAt,
            eventDate,
            description);
        
        created.AddDomainEvent(new MissingAnnouncementCreatedDomainEvent(created.Id, created.Location, created.CreatorId));
        
        return created;
    }

    public void Cancel(MissingAnnouncementDeleteReason reason, IAnimalAnnouncementDeletionPolicy deletionPolicy)
    {
        if (!deletionPolicy.CanDelete())
            throw new NotEnoughRightsException(IssueCodes.AccessDenied,
                "Вы не явлвяетесь создателем объявления");

        if (IsDeleted)
            throw new DomainException(IssueCodes.Announcement.AlreadyCancelled, "Объявление уже отменено");

        IsDeleted = true;
        DeleteReason = reason;
    }
    
    // EF
    private MissingAnnouncement(){}
}