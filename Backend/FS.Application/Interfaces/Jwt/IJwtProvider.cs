using FS.Core.UserDomain.Enums;

namespace FS.Application.Interfaces.Jwt;

public interface IJwtProvider
{
    string GenerateToken(Guid id, IEnumerable<Role> roles);
}
