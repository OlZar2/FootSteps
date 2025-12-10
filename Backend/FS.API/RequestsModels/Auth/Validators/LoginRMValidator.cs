using FluentValidation;
using FS.Contracts.Error;

namespace FS.API.RequestsModels.Auth.Validators;

public class LoginRMValidator : AbstractValidator<LoginRM>
{
    public LoginRMValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithErrorCode(IssueCodes.Required)
            .EmailAddress().WithErrorCode(IssueCodes.InvalidFormat);
        RuleFor(x => x.Password)
            .NotEmpty().WithErrorCode(IssueCodes.Required);
    }
}