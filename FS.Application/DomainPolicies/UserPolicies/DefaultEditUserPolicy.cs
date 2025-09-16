using FS.Contracts.Error;
using FS.Core.Entities;
using FS.Core.Exceptions;
using FS.Core.Policies.UserPolicies;

namespace FS.Application.DomainPolicies.UserPolicies;

public class DefaultEditUserPolicy : IEditUserPolicy
{
    public bool CanEdit(User target, Guid editor)
    {
        if (target.Id != editor)
            throw new NotEnoughRightsException(IssueCodes.AccessDenied,
                "Редактировать информацию может только сам пользователь");
        
        return true;
    }
}