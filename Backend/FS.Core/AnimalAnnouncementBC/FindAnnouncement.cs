using FS.Contracts.Error;
using FS.Core.AnimalAnnouncementBC.Enums;
using FS.Core.AnimalAnnouncementBC.Events;
using FS.Core.AnimalAnnouncementBC.Policies;
using FS.Core.Exceptions;
using FS.Core.ImageDomain.Entities;
using FS.Core.Shared.ValueObjects;

namespace FS.Core.AnimalAnnouncementBC;

public class FindAnnouncement : PetAnnouncement
{
    public FindAnnouncementDeleteReason DeleteReason { get; private set; }
    
    private FindAnnouncement(
        string? street,
        string? house,
        List<FSImage> images,
        Guid creatorId,
        string? district,
        PetType petType,
        Gender gender,
        string? color,
        string? breed,
        bool isCompleted,
        CoordinatesVO location,
        DateTime createdAt,
        DateTime eventDate,
        string? description)
        : base( 
            street:street,
            house:house,
            images: images,
            creatorId:creatorId,
            district:district,
            petType:petType,
            gender:gender,
            color:color,
            breed:breed,
            isCompleted:isCompleted,
            location:location,
            createdAt:createdAt,
            eventDate:eventDate,
            description:description) { }

    public static FindAnnouncement Create(
        string? street,
        string? house,
        List<FSImage> images,
        Guid creatorId,
        string? district,
        PetType petType,
        Gender gender,
        string? color,
        string? breed,
        CoordinatesVO location,
        DateTime eventDate,
        string? description)
    {
        var createdAt = DateTime.UtcNow;
        
        var created =  new FindAnnouncement(
            street: street,
            house: house,
            images: images,
            creatorId: creatorId,
            district: district,
            petType: petType,
            gender: gender,
            color: color,
            breed: breed,
            isCompleted: false,
            location: location,
            createdAt: createdAt,
            eventDate: eventDate,
            description: description);

        created.AddDomainEvent(new AnnouncementCreatedDomainEvent(created.Id));
        
        return created;
    }
    
    public void Cancel(FindAnnouncementDeleteReason reason, IAnimalAnnouncementDeletionPolicy deletionPolicy)
    {
        if (!deletionPolicy.CanDelete())
            throw new NotEnoughRightsException(IssueCodes.AccessDenied,
                "Вы не явлвяетесь создателем объявления");
        
        if (IsDeleted)
            throw new DomainException(IssueCodes.Announcement.AlreadyCancelled, "Объявление уже отменено");

        IsDeleted = true;
        DeleteReason = reason;
    }
    
    //EF
    private FindAnnouncement() { }
}