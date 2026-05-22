using FluentValidation;
using FS.Contracts.Error;

namespace FS.API.Controllers.User.RequestModels.Validators;

public class UpdateUserLocationRMValidator : AbstractValidator<UpdateUserLocationRM>
{
    public UpdateUserLocationRMValidator()
    {
        RuleFor(x => x.Latitude)
            .NotEmpty().WithErrorCode(IssueCodes.Required);
        RuleFor(x => x.Longitude)
            .NotEmpty().WithErrorCode(IssueCodes.Required);
    }
}