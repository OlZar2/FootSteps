using System.Security.Claims;
using FS.API.RequestsModels.User;
using FS.API.Services.ClaimLogic.Interfaces;
using FS.Application.DTOs.UserDTOs;
using FS.Application.Services.UserLogic.Interfaces;
using FS.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FS.API.Controllers;

[ApiController]
[Route("api/user")]
public class UserController(
    IUserService userService,
    IClaimService claimService) : ControllerBase
{
    [Authorize]
    [HttpPut("userInfo/{userId}")]
    public async Task<IActionResult> UpdateInfo(Guid userId, UpdateUserInfoRM updateInfo, CancellationToken ct)
    {
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
        
        return Ok();
    }
}