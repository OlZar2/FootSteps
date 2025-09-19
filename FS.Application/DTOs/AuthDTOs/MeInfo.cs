namespace FS.Application.DTOs.AuthDTOs;

public record MeInfo
{
    public required string FirstName { get; init; }
    public required string? SecondName { get; init; }
    public required string? Patronymic { get; init; }
    
    public required string? AvatarPath { get; init; }
}