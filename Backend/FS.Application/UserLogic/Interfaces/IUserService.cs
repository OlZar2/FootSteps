using FS.Application.AuthLogic.DTOs;
using FS.Application.Shared.DTOs;
using FS.Application.UserLogic.DTOs;

namespace FS.Application.UserLogic.Interfaces;

public interface IUserService
{
    Task UpdateUserInfoAsync(Guid actorId, UpdateUserInfo userInfo, CancellationToken ct);

    Task UpdateUserAvatarAsync(Guid actorId, UpdateUserAvatar updateUserAvatar, CancellationToken ct);

    Task UpdateUserLocation(Guid userId, CoordinatesDto coordinates, CancellationToken ct);
    
    Task AddDevice(Guid userId, string deviceToken, CancellationToken ct);
    
    Task<UserMainInfo> GetUserMainInfo(Guid userId, CancellationToken ct);
}