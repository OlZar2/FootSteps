using FluentValidation;
using FS.API.Errors;
using FS.API.RequestsModels.Auth;
using FS.Application.DTOs.AuthDTOs;
using FS.Application.Services.AuthLogic.Interfaces;
using FS.Contracts.Error;
using Microsoft.AspNetCore.Mvc;

namespace FS.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService, IValidator<RegisterRM> validator) : ControllerBase
{
    
    [HttpPost("/register")]
    [ProducesResponseType(typeof(ErrorEnvelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CreatedUserDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(InternalError), StatusCodes.Status500InternalServerError)]
    public async Task<CreatedUserDTO> Register([FromForm] RegisterRM request, CancellationToken ct)
    {
        await validator.ValidateAndThrowAsync(request, ct);
        
        await using var ms = new MemoryStream();
        await request.AvatarImage.CopyToAsync(ms, ct);

        var registerDTO = new RegisterDTO
        (
            request.Email,
            request.Password,
            request.FirstName,
            request.SecondName,
            request.Patronymic,
            request.Description,
            ms.ToArray()
        );
        
        var response = await authService.RegisterUserAsync(registerDTO, ct);
        return response;
    }
}