using System.Security.Claims;
using FluentValidation;
using FS.API.Controllers.User.RequestModels;
using FS.API.Services.ClaimLogic.Interfaces;
using FS.Application.AuthLogic.DTOs;
using FS.Application.Shared.DTOs;
using FS.Application.UserLogic.DTOs;
using FS.Application.UserLogic.Interfaces;
using FS.Core.UserDomain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FS.API.Controllers.User.Controllers;

/// <summary>
/// Методы для работы с информацией о пользователе
/// </summary>
[ApiController]
[Route("api/user")]
public class UserController(
    IUserService userService,
    IClaimService claimService,
    IValidator<UpdateUserInfoRM> updateUserInfoValidator,
    IValidator<UpdateUserLocationRM> updateUserLocationValidator,
    IValidator<AddDeviceRM> addDeviceValidator,
    IValidator<BlockUserRM> blockUserValidator) : ControllerBase
{
    /// <summary>
    /// Обновить информацию о пользователе
    /// </summary>
    [Authorize]
    [HttpPut("{userId:guid}/userInfo")]
    public async Task UpdateInfo(Guid userId, UpdateUserInfoRM updateInfo, CancellationToken ct)
    {
        await updateUserInfoValidator.ValidateAndThrowAsync(updateInfo, ct);
        
        var currentUserIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var currentUserId = claimService.TryParseGuidClaim(currentUserIdClaim);
        
        var userInfo = new UpdateUserInfo
        {
            UserId = userId,
            FirstName = updateInfo.FirstName,
            SecondName = updateInfo.SecondName,
            Patronymic = updateInfo.Patronymic,
            Description = updateInfo.Description,
            UserContacts = updateInfo.UserContacts?.Select(uc => new UserContactData
                {
                    ContactType = (ContactType)uc.ContactType,
                    Url = uc.Url
                })
                .ToArray(),
        };
        
        await userService.UpdateUserInfoAsync(currentUserId ,userInfo, ct);
    }

    /// <summary>
    /// Обновить аватар пользователя
    /// </summary>
    [Authorize]
    [HttpPut("{userId:guid}/avatar")]
    public async Task UpdateAvatar(Guid userId, [FromForm] UpdateUserAvatarRM request, CancellationToken ct)
    {
        var currentUserIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var currentUserId = claimService.TryParseGuidClaim(currentUserIdClaim);

        byte[]? avatarContent = null;

        if (request.AvatarImage != null)
        {
            await using var ms = new MemoryStream();
            await request.AvatarImage.CopyToAsync(ms, ct);
            avatarContent = ms.ToArray();
        }
        
        var fileInfo = avatarContent != null ? new FileData
        {
            Content = avatarContent
        } : null;

        var dto = new UpdateUserAvatar()
        {
            UserId = userId,
            Avatar = fileInfo
        };
        
        await userService.UpdateUserAvatarAsync(currentUserId, dto, ct);
    }
    
    /// <summary>
    /// Обновить последнее местоположение пользователя
    /// </summary>
    [Authorize]
    [HttpPut("location")]
    public async Task UpdateLastLocation(UpdateUserLocationRM request, CancellationToken ct)
    {
        await updateUserLocationValidator.ValidateAndThrowAsync(request, ct);
        
        var currentUserIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var currentUserId = claimService.TryParseGuidClaim(currentUserIdClaim);

        var coordinatesDto = new CoordinatesDto
        {
            Latitude = request.Latitude,
            Longitude = request.Longitude,
        };
        
        await userService.UpdateUserLocation(currentUserId, coordinatesDto, ct);
    }

    /// <summary>
    /// Добавить устройство пользователю
    /// </summary>
    [Authorize]
    [HttpPost("device")]
    public async Task AddDevice(AddDeviceRM addDevice, CancellationToken ct)
    {
        await addDeviceValidator.ValidateAndThrowAsync(addDevice, ct);
        
        var currentUserIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var currentUserId = claimService.TryParseGuidClaim(currentUserIdClaim);
        
        await userService.AddDevice(currentUserId, addDevice.DeviceToken!, ct);
    }

    /// <summary>
    /// Назначить пользователю роль администратора
    /// </summary>
    [Authorize(Roles = nameof(Role.Admin))]
    [HttpPost("{userId:guid}/roles/admin")]
    public async Task AssignAdminRole(Guid userId, CancellationToken ct)
    {
        await userService.AssignAdminRoleAsync(userId, ct);
    }

    /// <summary>
    /// Удалить у пользователя роль администратора
    /// </summary>
    [Authorize(Roles = nameof(Role.Admin))]
    [HttpDelete("{userId:guid}/roles/admin")]
    public async Task RemoveAdminRole(Guid userId, CancellationToken ct)
    {
        await userService.RemoveAdminRoleAsync(userId, ct);
    }

    /// <summary>
    /// Заблокировать пользователя
    /// </summary>
    [Authorize(Roles = nameof(Role.Admin))]
    [HttpPost("{userId:guid}/block")]
    public async Task BlockUser(Guid userId, BlockUserRM request, CancellationToken ct)
    {
        await blockUserValidator.ValidateAndThrowAsync(request, ct);

        await userService.BlockUserAsync(userId, request.Reason!, ct);
    }

    /// <summary>
    /// Разблокировать пользователя
    /// </summary>
    [Authorize(Roles = nameof(Role.Admin))]
    [HttpDelete("{userId:guid}/block")]
    public async Task UnblockUser(Guid userId, CancellationToken ct)
    {
        await userService.UnblockUserAsync(userId, ct);
    }

    /// <summary>
    /// Возвращает профиль пользователя
    /// </summary>
    /// <param name="userId">id пользователя</param>
    /// <param name="ct"></param>
    [Authorize]
    [HttpGet("{userId:guid}")]
    public async Task<UserMainInfo> GetProfile(Guid userId, CancellationToken ct) =>
        await userService.GetUserMainInfo(userId, ct);
}
