using FluentValidation;
using FS.API.Errors;
using FS.API.RequestsModels.Auth;
using FS.Application.DTOs.AuthDTOs;
using FS.Application.DTOs.Shared;
using FS.Application.Services.AuthLogic.Interfaces;
using FS.Contracts.Error;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FS.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService, IValidator<RegisterRM> registerValidator,
    IValidator<LoginRM> loginValidator) : ControllerBase
{
    
    [HttpPost("register")]
    [ProducesResponseType(typeof(CreatedUserData), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorEnvelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(InternalError), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Регистрация пользователя",
        Description = "Возвращает объект пользователя, если успешно"
    )]
    public async Task<CreatedUserData> Register([FromForm] RegisterRM request, CancellationToken ct)
    {
        await registerValidator.ValidateAndThrowAsync(request, ct);
        
        await using var ms = new MemoryStream();
        await request.AvatarImage.CopyToAsync(ms, ct);

        var registerDTO = new RegisterData
        (
            request.Email,
            request.Password,
            request.FirstName,
            request.SecondName,
            request.Patronymic,
            request.Description,
            new FileData
            {
                Content = ms.ToArray(),
            }
        );
        
        var response = await authService.RegisterUserAsync(registerDTO, ct);
        return response;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(JwtData), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorEnvelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(InternalError), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Логин",
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