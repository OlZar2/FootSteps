using FS.Contracts.Error;
using FS.Core.Enums;
using FS.Core.Exceptions;
using FS.Core.Policies.AnnouncementPolicies;
using NetTopologySuite.Geometries;

namespace FS.Core.Entities;

public class FindAnnouncement : PetAnnouncement
{
    public FindAnnouncementDeleteReason DeleteReason { get; private set; }
    
    private FindAnnouncement(
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
        List<Image> images,
        Guid creatorId,
        string? district,
        PetType petType,
        Gender gender,
        string? color,
        string? breed,
        Point location,
        DateTime eventDate,
        string? description)
    {
        var createdAt = DateTime.UtcNow;
        
        return new FindAnnouncement(
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