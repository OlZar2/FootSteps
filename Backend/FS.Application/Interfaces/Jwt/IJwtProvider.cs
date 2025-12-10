namespace FS.Application.Interfaces.Jwt;

public interface IJwtProvider
{
    string GenerateToken(Guid id);
}
