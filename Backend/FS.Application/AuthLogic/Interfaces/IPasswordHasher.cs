namespace FS.Application.AuthLogic.Interfaces;

public interface IPasswordHasher
{
    string GenerateHash(string password);
    void VerifyPassword(string password, string hashedPassword);
}
