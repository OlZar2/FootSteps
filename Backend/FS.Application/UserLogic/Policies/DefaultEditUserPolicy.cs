using FS.Contracts.Error;
using FS.Core.Exceptions;
using FS.Core.UserDomain;
using FS.Core.UserDomain.UserPolicies;

namespace FS.Application.UserLogic.Policies;

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