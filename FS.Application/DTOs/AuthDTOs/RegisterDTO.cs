using System.ComponentModel.DataAnnotations;

namespace FS.Application.DTOs.AuthDTOs;

public record RegisterDTO(
    string Email,
    string Password,
    string FirstName,
    string SecondName,
    string? Patronymic,
    string? Description,
    byte[] AvatarImage) { }
