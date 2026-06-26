using FS.Contracts.Error;
using FS.Core.Exceptions;
using FS.Core.ImageDomain.Entities;
using FS.Core.Shared.Abstractions;
using FS.Core.UserDomain.Entities;
using FS.Core.UserDomain.UserPolicies;
using FS.Core.UserDomain.ValueObjects;
using NetTopologySuite.Geometries;

namespace FS.Core.UserDomain;

public class User : AggregateRoot
{
    private readonly List<UserContact> _contacts = [];
    private readonly List<UserDevice> _userDevices = [];
    public IReadOnlyCollection<UserContact> Contacts => _contacts.AsReadOnly();
    public IReadOnlyCollection<UserDevice> UserDevices => _userDevices.AsReadOnly();
    
    public FullName FullName { get; private set; }
    
    public string? Description { get; private set; }
    
    public Guid? AvatarImageId { get; private set; }
    public FSImage? AvatarImage { get; private set; }
    
    public Email Email { get; private set; }
    
    public string PasswordHash { get; private set; }
    
    public Point? LastCoordinates { get; private set; }
    
    private User(
        Email email,
        string passwordHash,
        FullName fullName,
        string? description,
        FSImage? avatarImage,
        List<UserContact> contacts,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Email = email;
        PasswordHash = passwordHash;
        FullName = fullName;
        Description = description;
        AvatarImage = avatarImage;
        AvatarImageId = avatarImage?.Id;
        _contacts = contacts;
    }
    
    public static User Register(
        Email email,
        string passwordHash,
        FullName fullName,
        string? description,
        FSImage? avatarImage,
        InitialContact[] initialContacts,
        Guid? id = null)
    {
        if (string.IsNullOrEmpty(passwordHash)) throw new DomainException(IssueCodes.Required,
            "cannot be null or whitespace.", "password");

        EnsureUniqueKinds(initialContacts);
        var contacts = initialContacts.Select(ic => UserContact.Create(ic.ContactType, ic.Url)).ToList();

        return new User(
            email,
            passwordHash,
            fullName,
            description,
            avatarImage,
            contacts,
            id
        );
    }

    public void UpdateFullName(Guid editorId, FullName fullName, IEditUserPolicy editUserPolicy)
    {
        if(editUserPolicy.CanEdit(this, editorId))
            FullName = fullName;
    }

    public void UpdateDescription(Guid editorId, string description, IEditUserPolicy editUserPolicy)
    {
        if(editUserPolicy.CanEdit(this, editorId))
            Description = description;
    }

    public void UpdateContacts(Guid editorId, InitialContact[] initialContacts, IEditUserPolicy editUserPolicy)
    {
        if(!editUserPolicy.CanEdit(this, editorId))
            return;
        
        EnsureUniqueKinds(initialContacts);
        var contacts = initialContacts.Select(ic => UserContact.Create(ic.ContactType, ic.Url)).ToList();
        
        _contacts.Clear();
        _contacts.AddRange(contacts);
    }

    public void UpdateAvatar(Guid editorId, FSImage? avatarImage, IEditUserPolicy editUserPolicy)
    {
        if(!editUserPolicy.CanEdit(this, editorId))
            return;
        
        AvatarImage = avatarImage;
    }

    public void AddOrActivateDevice(UserDevice device)
    {
        var deviceToActivate = _userDevices.FirstOrDefault(ud => ud.DeviceToken == device.DeviceToken);
        if (deviceToActivate is null)
        {
            _userDevices.Add(device);
        }
        else
        {
            deviceToActivate.Activate();
        }
    }
    
    public void DeactivateDevice(UserDevice device)
    {
        var deviceToDeactivate = _userDevices.FirstOrDefault(ud => ud.DeviceToken == device.DeviceToken);
        deviceToDeactivate?.Deactivate();
    }
    
    public void UpdateLastCoordinates(Point coordinates)
    {
        LastCoordinates = coordinates;
    }

    private static void EnsureUniqueKinds(InitialContact[]? contacts)
    {
        if(contacts == null) return;
        
        var duplicates = contacts
            .GroupBy(c => c.ContactType)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicates.Count != 0)
        {
            throw new DomainException(
                IssueCodes.NotUnique,
                $"User cannot have more than one contact of kind(s): {string.Join(", ", duplicates)}",
                nameof(UserContact.Type));
        }
    }
    
    // EF
    private User() { }
}