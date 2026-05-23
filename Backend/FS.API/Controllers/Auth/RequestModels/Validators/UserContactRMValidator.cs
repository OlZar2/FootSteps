using FluentValidation;
using FS.Contracts.Error;
using FS.Core.UserDomain.Enums;

namespace FS.API.Controllers.Auth.RequestModels.Validators;

public class UserContactRMValidator : AbstractValidator<UserContactRM>
{
    public UserContactRMValidator()
    {
        RuleFor(x => x.ContactType)
            .NotNull().WithErrorCode(IssueCodes.Required)
            .Must(v => Enum.IsDefined(typeof(ContactType), v)).WithErrorCode(IssueCodes.InvalidValue);
        RuleFor(x => x.Url)
            .NotEmpty().WithErrorCode(IssueCodes.Required)
            .NotNull().WithErrorCode(IssueCodes.Required);
    }
}