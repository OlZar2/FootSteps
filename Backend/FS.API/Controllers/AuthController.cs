using System.Security.Claims;
using FluentValidation;
using FS.API.Errors;
using FS.API.RequestsModels.Auth;
using FS.API.Services.ClaimLogic.Interfaces;
using FS.Application.DTOs.AuthDTOs;
using FS.Application.DTOs.UserDTOs;
using FS.Application.Services.AuthLogic.Interfaces;
using FS.Contracts.Error;
using FS.Core.UserDomain.Enums;
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
    IClaimService claimService,
    ILogger<AuthController> logger) : ControllerBase
{
    /// <summary>
    /// Информация о текущем пользователе
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserMainInfo), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorEnvelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(InternalError), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Description = "Возвращает информацию о текущем пользователе"
    )]
    public async Task<UserMainInfo> GetMeAsync(CancellationToken ct)
    {
        var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var userId = claimService.TryParseGuidClaim(userIdClaim);
        
        return await authService.GetMeAsync(userId, ct);
    }
 
    /// <summary>
    /// Регистрация пользователя
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorEnvelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(InternalError), StatusCodes.Status500InternalServerError)]
    public async Task Register([FromForm] RegisterRM request, CancellationToken ct)
    {
        await registerValidator.ValidateAndThrowAsync(request, ct);

        var registerDTO = new RegisterData
        (
            request.Email,
            request.Password,
            request.FirstName,
            request.SecondName,
            request.Patronymic,
            request.Description,
            AvatarImageId: request.AvatarImageId,
            request.UserContacts?.Select(uc => new UserContactData
            {
                ContactType = (ContactType)uc.ContactType,
                Url = uc.Url,
            }).ToArray() ?? []
        );

        await authService.RegisterUserAsync(registerDTO, ct);
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
        logger.LogInformation("test");
        
        await loginValidator.ValidateAndThrowAsync(request, ct);
        
        var loginDTO = new LoginData(request.Email, request.Password);
        
        var response = await authService.LoginAsync(loginDTO, ct);
        return response;
    }
}