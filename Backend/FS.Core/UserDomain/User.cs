using FS.Contracts.Error;
using FS.Core.Exceptions;
using FS.Core.ImageDomain.Entities;
using FS.Core.Shared.Abstractions;
using FS.Core.UserDomain.Entities;
using FS.Core.UserDomain.Enums;
using FS.Core.UserDomain.Events;
using FS.Core.UserDomain.UserPolicies;
using FS.Core.UserDomain.ValueObjects;
using NetTopologySuite.Geometries;

namespace FS.Core.UserDomain;

public class User : AggregateRoot
{
    public const int MaxBlockReasonLength = 1000;

    private readonly List<UserContact> _contacts = [];
    private readonly List<UserDevice> _userDevices = [];
    private readonly List<UserRole> _roles = [];
    public IReadOnlyCollection<UserContact> Contacts => _contacts.AsReadOnly();
    public IReadOnlyCollection<UserDevice> UserDevices => _userDevices.AsReadOnly();
    public IReadOnlyCollection<UserRole> Roles => _roles.AsReadOnly();
    
    public FullName FullName { get; private set; }
    
    public string? Description { get; private set; }
    
    public Guid? AvatarImageId { get; private set; }
    public FSImage? AvatarImage { get; private set; }
    
    public Email Email { get; private set; }
    
    public string PasswordHash { get; private set; }

    public bool IsEmailConfirmed { get; private set; }

    public string? EmailConfirmationToken { get; private set; }

    public DateTime? EmailConfirmationLastSentAt { get; private set; }

    public bool IsBlocked { get; private set; }

    public string? BlockReason { get; private set; }
    
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
        IsEmailConfirmed = false;
        EmailConfirmationToken = Guid.NewGuid().ToString("N");
        EmailConfirmationLastSentAt = DateTime.UtcNow;
        _contacts = contacts;
        AssignRole(Role.User);
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

        var user = new User(
            email,
            passwordHash,
            fullName,
            description,
            avatarImage,
            contacts,
            id
        );
        
        user.AssignRole(Role.User);

        user.AddDomainEvent(new UserRegisteredDomainEvent(
            user.Id,
            user.Email.Value,
            user.EmailConfirmationToken!));

        return user;
    }

    public void ConfirmEmail(string confirmationToken)
    {
        if (IsEmailConfirmed)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(confirmationToken) || EmailConfirmationToken != confirmationToken)
        {
            throw new InvalidEmailConfirmationTokenException(Id, nameof(confirmationToken));
        }

        IsEmailConfirmed = true;
        EmailConfirmationToken = null;
        EmailConfirmationLastSentAt = null;
    }

    public void Block(string reason)
    {
        var normalizedReason = reason.Trim();

        if (string.IsNullOrWhiteSpace(normalizedReason))
        {
            throw new DomainException(IssueCodes.Required, "cannot be null or whitespace.", nameof(BlockReason));
        }

        if (normalizedReason.Length > MaxBlockReasonLength)
        {
            throw new DomainException(
                IssueCodes.TooLong,
                $"cannot be longer than {MaxBlockReasonLength}.",
                nameof(BlockReason));
        }

        IsBlocked = true;
        BlockReason = normalizedReason;
    }

    public void RequestEmailConfirmationResend(DateTime utcNow)
    {
        if (IsEmailConfirmed)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(EmailConfirmationToken))
        {
            EmailConfirmationToken = Guid.NewGuid().ToString("N");
        }

        if (EmailConfirmationLastSentAt.HasValue &&
            utcNow - EmailConfirmationLastSentAt.Value < TimeSpan.FromMinutes(1))
        {
            throw new DomainException(
                IssueCodes.TooMany,
                "email confirmation can be requested only once per minute.",
                nameof(EmailConfirmationLastSentAt));
        }

        EmailConfirmationLastSentAt = utcNow;

        AddDomainEvent(new EmailConfirmationRequestedDomainEvent(
            Id,
            Email.Value,
            EmailConfirmationToken));
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

    public void AssignRole(Role role)
    {
        if (_roles.Any(userRole => userRole.Role == role))
        {
            return;
        }

        _roles.Add(UserRole.Create(Id, role));
    }

    public void RemoveRole(Role role)
    {
        var userRole = _roles.FirstOrDefault(userRole => userRole.Role == role);
        if (userRole is null)
        {
            return;
        }

        _roles.Remove(userRole);
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
