namespace FS.Core.UserDomain.UserPolicies;

public interface IEditUserPolicy
{
    bool CanEdit(User target, Guid editor);
}