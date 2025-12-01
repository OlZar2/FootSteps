using FluentValidation;
using FS.Contracts.Error;

namespace FS.API.RequestsModels.User.Validators;

public class AddDeviceRMValidator : AbstractValidator<AddDeviceRM>
{
    public AddDeviceRMValidator()
    {
        RuleFor(x => x.DeviceToken)
            .NotEmpty().WithErrorCode(IssueCodes.Required);
    }
}