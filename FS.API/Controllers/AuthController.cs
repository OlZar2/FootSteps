using System.Security.Claims;
using FluentValidation;
using FS.API.Errors;
using FS.API.RequestsModels.Auth;
using FS.API.Services.ClaimLogic.Interfaces;
using FS.Application.DTOs.AuthDTOs;
using FS.Application.DTOs.Shared;
using FS.Application.DTOs.UserDTOs;
using FS.Application.Services.AuthLogic.Interfaces;
using FS.Contracts.Error;
using FS.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FS.API.Controllers;

/// <summary>
/// Методы авторизации
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController(
    IAuthService authService,
    IValidator<RegisterRM> registerValidator,
    IValidator<LoginRM> loginValidator,
    IClaimService claimService) : ControllerBase
{
    /// <summary>
    /// Информация о текущем пользователе
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(MeInfo), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorEnvelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(InternalError), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Description = "Возвращает информацию о текущем пользователе"
    )]
    public async Task<MeInfo> GetMeAsync(CancellationToken ct)
    {
        var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var userId = claimService.TryParseGuidClaim(userIdClaim);
        
        return await authService.GetMeAsync(userId, ct);
    }
 
    /// <summary>
    /// Регистрация пользователя
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(CreatedUserData), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorEnvelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(InternalError), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Description = "Возвращает объект пользователя, если успешно"
    )]
    public async Task<CreatedUserData> Register([FromForm] RegisterRM request, CancellationToken ct)
    {
        await registerValidator.ValidateAndThrowAsync(request, ct);

        byte[]? avatarContent = null;

        if (request.AvatarImage != null)
        {
            await using var ms = new MemoryStream();
            await request.AvatarImage.CopyToAsync(ms, ct);
            avatarContent = ms.ToArray();
        }

        var registerDTO = new RegisterData
        (
            request.Email,
            request.Password,
            request.FirstName,
            request.SecondName,
            request.Patronymic,
            request.Description,
            avatarContent != null 
                ? new FileData { Content = avatarContent }
                : null,
            request.UserContacts?.Select(uc => new UserContactData
            {
                ContactType = (ContactType)uc.ContactType,
                Url = uc.Url,
            }).ToArray() ?? []
        );

        var response = await authService.RegisterUserAsync(registerDTO, ct);
        return response;
    }

    /// <summary>
    /// Логин
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(JwtData), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorEnvelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(InternalError), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Description = "Возвращает токен, если успешно"
    )]
    public async Task<JwtData> Login(LoginRM request, CancellationToken ct)
    {
        await loginValidator.ValidateAndThrowAsync(request, ct);
        
        var loginDTO = new LoginData(request.Email, request.Password);
        
        var response = await authService.LoginAsync(loginDTO, ct);
        return response;
    }
}