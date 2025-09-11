using System.ComponentModel;
using FS.Core.Entities;

namespace FS.Application.DTOs.AuthDTOs;

/// <summary>
/// DTO созданного пользователя.
/// </summary>
public record CreatedUserDTO
{
    /// <summary>
    /// Имя пользователя.
    /// </summary>
    public string FirstName { get; init; }

    /// <summary>
    /// Фамилия пользователя.
    /// </summary>
    public string LastName { get; init; }

    /// <summary>
    /// Отчество пользователя (необязательно).
    /// </summary>
    [Description("Zero-based page index")]
    public string? Patronymic { get; init; }

    /// <summary>
    /// Email пользователя.
    /// </summary>
    public string? Email { get; init; }

    /// <summary>
    /// Путь к аватару пользователя (если есть).
    /// </summary>
    public string? AvatarPath { get; init; }

    public static CreatedUserDTO From(User user) => new()
    {
        FirstName  = user.FullName.FirstName,
        LastName   = user.FullName.SecondName,
        Patronymic = user.FullName.Patronymic,
        Email      = user.Email.Value,
        AvatarPath = user.AvatarImage?.Path
    };
}