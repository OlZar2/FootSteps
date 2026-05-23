using FluentValidation;
using FS.Application.ImageLogic.Configurations;
using FS.Contracts.Error;
using Microsoft.Extensions.Options;

namespace FS.API.Controllers.Auth.RequestModels.Validators;

public class RegisterRMValidator : AbstractValidator<RegisterRM>
{
    public RegisterRMValidator(IOptions<ImagesOptions> imagesOptions)
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithErrorCode(IssueCodes.Required)
            .EmailAddress().WithErrorCode(IssueCodes.InvalidFormat);
        RuleFor(x => x.Password)
            .NotEmpty().WithErrorCode(IssueCodes.Required);
        RuleFor(x => x.FirstName)
            .NotEmpty().WithErrorCode(IssueCodes.Required)
            .MaximumLength(30).WithErrorCode(IssueCodes.TooLong);
        RuleFor(x => x.SecondName)
            .MaximumLength(40).WithErrorCode(IssueCodes.TooLong);
        RuleFor(x => x.Patronymic)
            .MaximumLength(50).WithErrorCode(IssueCodes.TooLong);
        RuleForEach(x => x.UserContacts)
            .SetValidator(new UserContactRMValidator());
    }
}