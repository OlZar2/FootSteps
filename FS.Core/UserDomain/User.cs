using FS.Contracts.Error;
using FS.Core.Exceptions;
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
    
    public Email Email { get; private set; }
    
    public string PasswordHash { get; private set; }
    
    public Point? LastCoordinates { get; private set; }
    
    private User(
        Email email,
        string passwordHash,
        FullName fullName,
        string? description,
        Guid? avatarImageId,
        List<UserContact> contacts,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Email = email;
        PasswordHash = passwordHash;
        FullName = fullName;
        Description = description;
        AvatarImageId = avatarImageId;
        _contacts = contacts;
    }
    
    public static User Register(
        Email email,
        string passwordHash,
        FullName fullName,
        string? description,
        Guid? avatarImageId,
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
            avatarImageId,
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

    public void UpdateAvatar(Guid editorId, Guid? avatarImageId, IEditUserPolicy editUserPolicy)
    {
        if(!editUserPolicy.CanEdit(this, editorId))
            return;
        
        AvatarImageId = avatarImageId;
    }

    public void AddDevice(UserDevice device)
    {
        if (!_userDevices.Select(ud => ud.DeviceToken).Contains(device.DeviceToken))
        {
            _userDevices.Add(device);
        }
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