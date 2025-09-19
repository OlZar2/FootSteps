using FS.Application.DTOs.AuthDTOs;
using FS.Application.Exceptions;
using FS.Application.Interfaces;
using FS.Application.Interfaces.QueryServices;
using FS.Application.Services.AuthLogic.Exceptions;
using FS.Application.Services.AuthLogic.Interfaces;
using FS.Application.Services.ImageLogic.Interfaces;
using FS.Core.Entities;
using FS.Core.Services;
using FS.Core.Stores;
using FS.Core.ValueObjects;
using FS.Core.ValueObjects.Contacts;
using HW.Application.Services.AuthLogic.Interfaces;

namespace FS.Application.Services.AuthLogic.Implementations;

public class AuthService(
    IJwtProvider jwtProvider,
    IPasswordHasher passwordHasher,
    IUserRepository userRepository,
    IImageService imageService,
    ITransactionService transactionService,
    IEmailUniqueService emailUniqueService,
    IUserQueryService userQueryService)
    : IAuthService
{
    public async Task<CreatedUserData> RegisterUserAsync(RegisterData userRegisterData, CancellationToken ct)
    {
        return await transactionService.ExecuteInTransactionAsync(async () =>
        {
            var emailVo = Email.Create(userRegisterData.Email);
            var fullNameVo = FullName
                .Create(userRegisterData.FirstName, userRegisterData.SecondName, userRegisterData.Patronymic);
            var hashedPassword = passwordHasher.GenerateHash(userRegisterData.Password);
            
            var image = userRegisterData.AvatarImage != null ? await imageService
                .CreateImageAsync(userRegisterData.AvatarImage.Content, ct, nameof(RegisterData.AvatarImage)) : null;
            
            var userInitContacts =
                userRegisterData.UserContacts?.Select(uc => new InitialContact(uc.ContactType, uc.Url))
                    .ToArray() ?? [];

            var user = await User.RegisterAsync(
                emailVo,
                hashedPassword,
                fullNameVo,
                userRegisterData.Description,
                image,
                emailUniqueService,
                userInitContacts, ct);

            await userRepository.CreateAsync(user, ct);

            return CreatedUserData.From(user);
        }, ct);
    }

    public async Task<JwtData> LoginAsync(LoginData loginData, CancellationToken ct)
    {
        try
        {
            var account = await userRepository.GetByEmailAsync(loginData.Email, ct);
            passwordHasher.VerifyPassword(loginData.Password, account.PasswordHash);

            var token = jwtProvider.GenerateToken(account.Id);
            return new JwtData(token);
        }
        catch (NotFoundException)
        {
            throw new WrongPasswordException();
        }
    }

    public async Task<MeInfo> GetMeAsync(Guid userId, CancellationToken ct)
    {
        var info = await userQueryService.GetUserMainInfoByIdAsync(userId, ct);
        return info;
    }
}