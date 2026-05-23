using FS.Application.UserLogic.DTOs;

namespace FS.Application.AuthLogic.DTOs;

public record RegisterData(
    string Email,
    string Password,
    string FirstName,
    string? SecondName,
    string? Patronymic,
    string? Description,
    Guid? AvatarImageId,
    UserContactData[] UserContacts) { }
