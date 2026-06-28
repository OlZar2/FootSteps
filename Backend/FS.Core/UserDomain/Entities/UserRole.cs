using FS.Contracts.Error;
using FS.Core.Exceptions;
using FS.Core.UserDomain.Enums;

namespace FS.Core.UserDomain.Entities;

public class UserRole
{
    public Guid UserId { get; private set; }

    public Role Role { get; private set; }

    private UserRole(Guid userId, Role role)
    {
        if (!Enum.IsDefined(role))
        {
            throw new DomainException(IssueCodes.InvalidValue, "role is invalid.", nameof(role));
        }

        UserId = userId;
        Role = role;
    }

    public static UserRole Create(Guid userId, Role role) => new(userId, role);

    // EF
    private UserRole() { }
}
