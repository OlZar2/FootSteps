using FS.Contracts.Error;
using FS.Core.Enums;
using FS.Core.Exceptions;
using FS.Core.Policies.AnnouncementPolicies;
using FS.Core.ValueObjects;
using NetTopologySuite.Geometries;

namespace FS.Core.Entities;

public class MissingAnnouncement : PetAnnouncement
{
    public string PetName { get; private set; }
    
    public MissingAnnouncementDeleteReasons DeleteReason { get; private set; }
    
    private MissingAnnouncement(
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
        string petName,
        DateTime createdAt,
        DateTime eventDate)
        : base(fullPlace, images, creatorId, district, petType,  gender, color, breed, isCompleted, location, createdAt, eventDate)
    {
        PetName = petName;
    }

    public static MissingAnnouncement Create(
        Place fullPlace,
        List<Image> images,
        Guid creatorId,
        District district,
        PetType petType,
        Gender gender,
        string? color,
        string? breed,
        Point location,
        string petName,
        DateTime eventDate)
    {
        var createdAt = DateTime.UtcNow;
        
        return new MissingAnnouncement(
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
            petName,
            createdAt,
            eventDate);
    }

    public void Delete(MissingAnnouncementDeleteReasons reason, IAnimalAnnouncementDeletionPolicy deletionPolicy)
    {
        if (!deletionPolicy.CanDelete())
            throw new NotEnoughRightsException(IssueCodes.AccessDenied,
                "Вы не явлвяетесь создателем объявления");

        IsDeleted = true;
        DeleteReason = reason;
    }
    
    // EF
    private MissingAnnouncement(){}
}