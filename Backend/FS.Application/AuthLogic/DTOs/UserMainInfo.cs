namespace FS.Application.AuthLogic.DTOs;

public record UserMainInfo
{
    public required Guid Id { get; init; }
    
    public required string FirstName { get; init; }
    public required string? SecondName { get; init; }
    public required string? Patronymic { get; init; }
    
    public required string? AvatarPath { get; init; }
    
    public required ContactData[] Contacts { get; init; }
    
    public required string? Description { get; init; }
}