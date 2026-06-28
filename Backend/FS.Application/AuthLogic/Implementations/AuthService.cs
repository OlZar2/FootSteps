using FS.Application.AuthLogic.DTOs;
using FS.Application.AuthLogic.Exceptions;
using FS.Application.AuthLogic.Interfaces;
using FS.Application.Interfaces.Jwt;
using FS.Application.Interfaces.QueryServices;
using FS.Application.Interfaces.Transaction;
using FS.Application.Shared.Exceptions;
using FS.Contracts.Error;
using FS.Core.AnimalAnnouncementBC.Stores;
using FS.Core.Exceptions;
using FS.Core.ImageDomain.Entities;
using FS.Core.UserDomain;
using FS.Core.UserDomain.Stores;
using FS.Core.UserDomain.ValueObjects;

namespace FS.Application.AuthLogic.Implementations;

public class AuthService(
    IJwtProvider jwtProvider,
    IPasswordHasher passwordHasher,
    IUserRepository userRepository,
    ITransactionFactory transactionFactory,
    IUserQueryService userQueryService,
    IImageRepository imageRepository)
    : IAuthService
{
    public async Task RegisterUserAsync(RegisterData userRegisterData, CancellationToken ct)
    {
        await using var transaction = await transactionFactory.BeginAsync(ct);
        
        var isEmailUnique = await userRepository.IsEmailUnique(userRegisterData.Email, ct);
        if(!isEmailUnique) throw new DomainException(IssueCodes.NotUnique,
            "email must be unique.", nameof(userRegisterData.Email));
        
        var emailVo = Email.Create(userRegisterData.Email);
        var fullNameVo = FullName
            .Create(userRegisterData.FirstName, userRegisterData.SecondName, userRegisterData.Patronymic);
        var hashedPassword = passwordHasher.GenerateHash(userRegisterData.Password);

        FSImage? image = null;
        if (userRegisterData.AvatarImageId != null)
        {
            var s3Key = Guid.NewGuid().ToString();
            image = await imageRepository.GetByIdAsync(userRegisterData.AvatarImageId.Value, ct);
        }
        
        var userInitContacts =
            userRegisterData.UserContacts.Select(uc => new InitialContact(uc.ContactType, uc.Url))
                .ToArray();

        var user = User.Register(
            emailVo,
            hashedPassword,
            fullNameVo,
            userRegisterData.Description,
            image,
            userInitContacts);

        await userRepository.CreateAsync(user, ct);
        
        await transaction.CommitAsync(ct);
    }

    public async Task<JwtData> LoginAsync(LoginData loginData, CancellationToken ct)
    {
        try
        {
            var account = await userRepository.GetByEmailAsync(loginData.Email, ct);
            passwordHasher.VerifyPassword(loginData.Password, account.PasswordHash);

            if (!account.IsEmailConfirmed)
            {
                throw new EmailNotConfirmedException();
            }

            var token = jwtProvider.GenerateToken(account.Id, account.Roles.Select(userRole => userRole.Role));
            return new JwtData(token);
        }
        catch (NotFoundException)
        {
            throw new WrongPasswordException();
        }
    }

    public async Task ResendEmailConfirmationAsync(string email, CancellationToken ct)
    {
        await using var transaction = await transactionFactory.BeginAsync(ct);

        var user = await userRepository.GetByEmailForUpdateAsync(email, ct);
        user.RequestEmailConfirmationResend(DateTime.UtcNow);

        await userRepository.UpdateAsync(user, ct);

        await transaction.CommitAsync(ct);
    }

    public async Task ConfirmEmailAsync(Guid userId, string token, CancellationToken ct)
    {
        await using var transaction = await transactionFactory.BeginAsync(ct);

        var user = await userRepository.GetByIdAsync(userId, ct)
                   ?? throw new UserForEmailConfirmationNotFoundException(userId);
        
        user.ConfirmEmail(token);
        
        await userRepository.UpdateAsync(user, ct);

        await transaction.CommitAsync(ct);
    }

    public async Task<UserMainInfo> GetMeAsync(Guid userId, CancellationToken ct)
    {
        var info = await userQueryService.GetUserMainInfoByIdAsync(userId, ct);
        return info;
    }
}
