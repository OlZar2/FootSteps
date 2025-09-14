using FS.Application.DTOs.UserDTOs;
using FS.Core.Entities;

namespace FS.Application.DTOs.AuthDTOs;

public record CreatedUserData
{
    public Guid Id { get; init; }
    
    public required string FirstName { get; init; }
    
    public required string SecondName { get; init; }
    
    public string? Patronymic { get; init; }
    
    public string? Email { get; init; }
    
    public string? AvatarPath { get; init; }

    public UserContactData[] Contacts { get; init; } = [];

    public static CreatedUserData From(User user) => new()
    {
        Id = user.Id,
        FirstName  = user.FullName.FirstName,
        SecondName   = user.FullName.SecondName,
        Patronymic = user.FullName.Patronymic,
        Email      = user.Email.Value,
        AvatarPath = user.AvatarImage?.Path,
        Contacts = user.Contacts.Select(UserContactData.From).ToArray(),
    };
}