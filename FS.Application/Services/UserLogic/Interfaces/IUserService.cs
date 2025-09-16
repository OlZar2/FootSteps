using FS.Application.DTOs.UserDTOs;

namespace FS.Application.Services.UserLogic.Interfaces;

public interface IUserService
{
    Task UpdateUserInfoAsync(Guid actorId, UpdateUserInfo userInfo, CancellationToken ct);

    Task UpdateUserAvatarAsync(Guid actorId, UpdateUserAvatar updateUserAvatar, CancellationToken ct);
}