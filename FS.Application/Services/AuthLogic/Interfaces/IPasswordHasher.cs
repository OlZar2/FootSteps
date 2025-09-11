namespace HW.Application.Services.AuthLogic.Interfaces;

public interface IPasswordHasher
{
    string GenerateHash(string password);
    void VerifyPassword(string password, string hashedPassword);
}
