using System.Security.Claims;
using FluentValidation;
using FS.API.RequestsModels.User;
using FS.API.Services.ClaimLogic.Interfaces;
using FS.Application.DTOs.Shared;
using FS.Application.DTOs.UserDTOs;
using FS.Application.Services.UserLogic.Interfaces;
using FS.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FS.API.Controllers;

/// <summary>
/// Методы для работы с информацией о пользователе
/// </summary>
[ApiController]
[Route("api/user")]
public class UserController(
    IUserService userService,
    IClaimService claimService,
    IValidator<UpdateUserInfoRM> updateUserInfoValidator,
    IValidator<UpdateUserAvatarRM> updateUserAvatarValidator) : ControllerBase
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
    public async Task UpdateAvatar(Guid userId, UpdateUserAvatarRM request, CancellationToken ct)
    {
        await updateUserAvatarValidator.ValidateAndThrowAsync(request, ct);
        
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
}