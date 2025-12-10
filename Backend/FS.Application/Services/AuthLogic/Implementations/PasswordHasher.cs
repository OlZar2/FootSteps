using FS.Application.Services.AuthLogic.Exceptions;
using FS.Application.Services.AuthLogic.Interfaces;

namespace FS.Application.Services.AuthLogic.Implementations;

public class PasswordHasher : IPasswordHasher
{
    public string GenerateHash(string password) =>
        BCrypt.Net.BCrypt.EnhancedHashPassword(password);

    public void VerifyPassword(string password, string hashedPassword)
    {
        var isPasswordVerified = BCrypt.Net.BCrypt.EnhancedVerify(password, hashedPassword);
        if (!isPasswordVerified)
            throw new WrongPasswordException();
    }
}
