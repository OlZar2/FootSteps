using FS.Application.DTOs.UserDTOs;

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
}