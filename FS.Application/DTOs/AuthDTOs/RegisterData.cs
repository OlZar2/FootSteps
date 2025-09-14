using FS.Application.DTOs.Shared;
using FS.Application.DTOs.UserDTOs;

namespace FS.Application.DTOs.AuthDTOs;

public record RegisterData(
    string Email,
    string Password,
    string FirstName,
    string SecondName,
    string? Patronymic,
    string? Description,
    FileData? AvatarImage,
    UserContactData[] UserContacts) { }
