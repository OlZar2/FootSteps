using FluentValidation;
using FS.Contracts.Error;
using DomainUser = FS.Core.UserDomain.User;

namespace FS.API.Controllers.User.RequestModels.Validators;

public class BlockUserRMValidator : AbstractValidator<BlockUserRM>
{
    public BlockUserRMValidator()
    {
        RuleFor(x => x.Reason)
            .NotEmpty().WithErrorCode(IssueCodes.Required)
            .Must(reason => !string.IsNullOrWhiteSpace(reason)).WithErrorCode(IssueCodes.Required)
            .MaximumLength(DomainUser.MaxBlockReasonLength).WithErrorCode(IssueCodes.TooLong);
    }
}
