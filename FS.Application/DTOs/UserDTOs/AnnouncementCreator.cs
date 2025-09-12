using FS.Core.Entities;

namespace FS.Application.DTOs.UserDTOs;

public record AnnouncementCreator
{
    public Guid Id { get; init; }
    
    public required string FirstName { get; init; }
    public required string SecondName { get; init; }
    public string? Patronymic { get; init; }
    
    public required string? AvatarPath { get; init; }
    
    public static AnnouncementCreator From(User user) => new()
    {
        Id = user.Id,
        FirstName  = user.FullName.FirstName,
        SecondName   = user.FullName.SecondName,
        Patronymic = user.FullName.Patronymic,
        AvatarPath = user.AvatarImage?.Path
    };
}