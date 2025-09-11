using System.ComponentModel.DataAnnotations;
using FS.Contracts.Error;
using FS.Core.Exceptions;
using FS.Core.Services;
using FS.Core.ValueObjects;

namespace FS.Core.Entities;

public class User
{
    public Guid Id { get; private set; }
    
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
        Image? avatarImage
    )
    {
        Id = Guid.NewGuid();
        Email = email;
        PasswordHash = passwordHash;
        FullName = fullName;
        Description = description;
        AvatarImage = avatarImage;
        AvatarImageId = avatarImage?.Id;
    }
    
    public static async Task<User> RegisterAsync(
        Email email,
        string passwordHash,
        FullName fullName,
        string? description,
        Image? avatarImage,
        IEmailUniqueService emailUniqueService,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(passwordHash)) throw new DomainException(IssueCodes.Required,
            "cannot be null or whitespace.", "password");

        var isEmailUnique = await emailUniqueService.IsEmailUniqueAsync(email.Value, cancellationToken);
        if(!isEmailUnique) throw new DomainException(IssueCodes.NotUnique,
            "email must be unique.", nameof(email));

        return new User(
            email,
            passwordHash,
            fullName,
            description,
            avatarImage
        );
    }
    
    // EF
    private User() { }
}