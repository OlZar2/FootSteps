using FS.Application.DTOs.AuthDTOs;
using FS.Application.Exceptions;
using FS.Application.Interfaces;
using FS.Application.Services.AuthLogic.Exceptions;
using FS.Application.Services.AuthLogic.Interfaces;
using FS.Application.Services.ImageLogic.Interfaces;
using FS.Core.Entities;
using FS.Core.Services;
using FS.Core.Stores;
using FS.Core.ValueObjects;
using HW.Application.Services.AuthLogic.Interfaces;

namespace FS.Application.Services.AuthLogic.Implementations;

public class AuthService(
    IJwtProvider jwtProvider,
    IPasswordHasher passwordHasher,
    IUserRepository userRepository,
    IImageService imageService,
    ITransactionService transactionService,
    IEmailUniqueService emailUniqueService)
    : IAuthService
{
    public async Task<CreatedUserDTO> RegisterUserAsync(RegisterDTO userRegisterDTO, CancellationToken ct)
    {
        return await transactionService.ExecuteInTransactionAsync(async () =>
        {
            var emailVo = Email.Create(userRegisterDTO.Email);
            var fullNameVo = FullName
                .Create(userRegisterDTO.FirstName, userRegisterDTO.SecondName, userRegisterDTO.Patronymic);
            var hashedPassword = passwordHasher.GenerateHash(userRegisterDTO.Password);

            var image = await imageService.CreateImageAsync(userRegisterDTO.AvatarImage, nameof(RegisterDTO.AvatarImage));

            var user = await User.RegisterAsync(
                emailVo,
                hashedPassword,
                fullNameVo,
                userRegisterDTO.Description,
                image,
                emailUniqueService, ct);

            await userRepository.CreateAsync(user, ct);

            return CreatedUserDTO.From(user);
        }, ct);
    }

    public async Task<JwtDTO> LoginAsync(LoginDTO loginDTO, CancellationToken ct)
    {
        try
        {
            var account = await userRepository.GetByEmailAsync(loginDTO.Email, ct);
            passwordHasher.VerifyPassword(loginDTO.Password, account.PasswordHash);

            var token = jwtProvider.GenerateToken(account.Id);
            return new JwtDTO(token);
        }
        catch (NotFoundException)
        {
            throw new WrongPasswordException();
        }
    }
}