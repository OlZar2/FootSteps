using FS.Contracts.Error;
using FS.Core.Exceptions;
using FS.Core.Services;
using FS.Core.ValueObjects;
using FS.Core.ValueObjects.Contacts;

namespace FS.Core.Entities;

public class User
{
    public Guid Id { get; private set; }
    
    private readonly List<UserContact> _contacts = [];
    public IReadOnlyCollection<UserContact> Contacts => _contacts.AsReadOnly();
    
    public FullName FullName { get; private set; }
    
    public string? Description { get; private set; }
    
    public Guid? AvatarImageId { get; set; }
    public Image? AvatarImage { get; private set; }
    
    public Email Email { get; private set; }
    
    public string PasswordHash { get; private set; }
    
    private User(
        Email email,
        string passwordHash,
        FullName fullName,
        string? description,
        Image? avatarImage,
        List<UserContact> contacts)
    {
        Id = Guid.NewGuid();
        Email = email;
        PasswordHash = passwordHash;
        FullName = fullName;
        Description = description;
        AvatarImage = avatarImage;
        AvatarImageId = avatarImage?.Id;
        _contacts = contacts;
    }
    
    public static async Task<User> RegisterAsync(
        Email email,
        string passwordHash,
        FullName fullName,
        string? description,
        Image? avatarImage,
        IEmailUniqueService emailUniqueService,
        InitialContact[] initialContacts,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(passwordHash)) throw new DomainException(IssueCodes.Required,
            "cannot be null or whitespace.", "password");

        var isEmailUnique = await emailUniqueService.IsEmailUniqueAsync(email.Value, cancellationToken);
        if(!isEmailUnique) throw new DomainException(IssueCodes.NotUnique,
            "email must be unique.", nameof(email));

        EnsureUniqueKinds(initialContacts);
        var contacts = initialContacts.Select(ic => UserContact.Create(ic.Type, ic.Url)).ToList();

        return new User(
            email,
            passwordHash,
            fullName,
            description,
            avatarImage,
            contacts
        );
    }

    private static void EnsureUniqueKinds(InitialContact[]? contacts)
    {
        if(contacts == null) return;
        
        var duplicates = contacts
            .GroupBy(c => c.Type)
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