using FS.Application.AuthLogic.DTOs;

namespace FS.Application.UserLogic.DTOs;

public record AnnouncementCreator
{
    public Guid Id { get; init; }
    
    public required string FirstName { get; init; }
    public string? SecondName { get; init; }
    public string? Patronymic { get; init; }
    
    public required string? AvatarPath { get; init; }
    
    public string? Description { get; init; }
    
    public required ContactData[] Contacts { get; init; }
}