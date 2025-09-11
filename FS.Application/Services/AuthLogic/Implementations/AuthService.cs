using FS.Application.DTOs.AuthDTOs;
using FS.Application.Interfaces;
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
            var fullNameVo = FullName.Create(userRegisterDTO.FirstName, userRegisterDTO.SecondName, userRegisterDTO.Patronymic);
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
}