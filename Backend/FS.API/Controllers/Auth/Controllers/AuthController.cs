using System.Security.Claims;
using FluentValidation;
using FS.API.Controllers.Auth.RequestModels;
using FS.API.Errors;
using FS.API.Pages;
using FS.API.Services.ClaimLogic.Interfaces;
using FS.Application.AuthLogic.DTOs;
using FS.Application.AuthLogic.Interfaces;
using FS.Application.UserLogic.DTOs;
using FS.Contracts.Error;
using FS.Core.UserDomain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FS.API.Controllers.Auth.Controllers;

/// <summary>
/// Методы авторизации
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController(
    IAuthService authService,
    IValidator<RegisterRM> registerValidator,
    IValidator<LoginRM> loginValidator,
    IValidator<ResendEmailConfirmationRM> resendEmailConfirmationValidator,
    IClaimService claimService) : ControllerBase
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
    /// Повторная отправка письма для подтверждения почты
    /// </summary>
    [HttpPost("resend-email-confirmation")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorEnvelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorEnvelope), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(InternalError), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Description = "Повторно отправляет письмо со ссылкой для подтверждения почты. Для одного пользователя доступно не чаще одного раза в минуту."
    )]
    public async Task ResendEmailConfirmation(ResendEmailConfirmationRM request, CancellationToken ct)
    {
        await resendEmailConfirmationValidator.ValidateAndThrowAsync(request, ct);

        await authService.ResendEmailConfirmationAsync(request.Email, ct);
    }

    /// <summary>
    /// Подтверждение почты пользователя
    /// </summary>
    [HttpGet("confirm-email")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorEnvelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(InternalError), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ConfirmEmail([FromQuery] Guid userId, [FromQuery] string token, CancellationToken ct)
    {
        await authService.ConfirmEmailAsync(userId, token, ct);

        return Redirect(EmailConfirmationPageRoutes.Success);
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
