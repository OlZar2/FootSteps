using FluentValidation;
using FS.API.Controllers.Auth.RequestModels.Validators;
using FS.Contracts.Error;

namespace FS.API.Controllers.User.RequestModels.Validators;

public class UpdateUserInfoRMValidator : AbstractValidator<UpdateUserInfoRM>
{
    public UpdateUserInfoRMValidator()
    {
        RuleFor(x => x.FirstName)
            .MaximumLength(30).WithErrorCode(IssueCodes.TooLong);
        RuleFor(x => x.SecondName)
            .MaximumLength(40).WithErrorCode(IssueCodes.TooLong);
        RuleFor(x => x.Patronymic)
            .MaximumLength(50).WithErrorCode(IssueCodes.TooLong);
        RuleForEach(x => x.UserContacts)
            .SetValidator(new UserContactRMValidator());
    }
}