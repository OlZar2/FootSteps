namespace FS.Application.Interfaces;

public interface IJwtProvider
{
    string GenerateToken(Guid id);
}
