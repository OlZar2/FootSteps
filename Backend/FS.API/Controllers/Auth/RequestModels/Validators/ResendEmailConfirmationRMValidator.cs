using FluentValidation;
using FS.Contracts.Error;

namespace FS.API.Controllers.Auth.RequestModels.Validators;

public class ResendEmailConfirmationRMValidator : AbstractValidator<ResendEmailConfirmationRM>
{
    public ResendEmailConfirmationRMValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithErrorCode(IssueCodes.Required)
            .EmailAddress().WithErrorCode(IssueCodes.InvalidFormat);
    }
}
