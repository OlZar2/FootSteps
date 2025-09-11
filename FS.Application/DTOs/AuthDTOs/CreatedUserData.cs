using System.ComponentModel;
using FS.Core.Entities;

namespace FS.Application.DTOs.AuthDTOs;

public record CreatedUserData
{
    public string FirstName { get; init; }
    
    public string LastName { get; init; }
    
    public string? Patronymic { get; init; }
    
    public string? Email { get; init; }
    
    public string? AvatarPath { get; init; }

    public static CreatedUserData From(User user) => new()
    {
        FirstName  = user.FullName.FirstName,
        LastName   = user.FullName.SecondName,
        Patronymic = user.FullName.Patronymic,
        Email      = user.Email.Value,
        AvatarPath = user.AvatarImage?.Path
    };
}