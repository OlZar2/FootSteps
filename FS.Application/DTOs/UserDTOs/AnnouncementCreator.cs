using FS.Core.AnimalAnnouncementBC.Entities;
using FS.Core.UserDomain;

namespace FS.Application.DTOs.UserDTOs;

public record AnnouncementCreator
{
    public Guid Id { get; init; }
    
    public required string FirstName { get; init; }
    public string? SecondName { get; init; }
    public string? Patronymic { get; init; }
    
    public required string? AvatarPath { get; init; }

    public static AnnouncementCreator FromUserAndAvatar(User user, AnimalAnnouncementImage avatar)
    {
        return new AnnouncementCreator
        {
            Id = user.Id,
            FirstName = user.FullName.FirstName,
            SecondName = user.FullName.FirstName,
            Patronymic = user.FullName.FirstName,
            AvatarPath = avatar.FullImagePath
        };
    }
}