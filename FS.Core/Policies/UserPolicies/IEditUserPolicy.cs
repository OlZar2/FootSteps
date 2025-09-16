using FS.Core.Entities;

namespace FS.Core.Policies.UserPolicies;

public interface IEditUserPolicy
{
    bool CanEdit(User target, Guid editor);
}