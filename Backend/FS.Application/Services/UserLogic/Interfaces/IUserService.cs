using FS.Application.DTOs.AuthDTOs;
using FS.Application.DTOs.Shared;
using FS.Application.DTOs.UserDTOs;

namespace FS.Application.Services.UserLogic.Interfaces;

public interface IUserService
{
    Task UpdateUserInfoAsync(Guid actorId, UpdateUserInfo userInfo, CancellationToken ct);

    Task UpdateUserAvatarAsync(Guid actorId, UpdateUserAvatar updateUserAvatar, CancellationToken ct);

    Task UpdateUserLocation(Guid userId, CoordinatesDto coordinates, CancellationToken ct);
    
    Task AddDevice(Guid userId, string deviceToken, CancellationToken ct);
    
    Task<UserMainInfo> GetUserMainInfo(Guid userId, CancellationToken ct);
}