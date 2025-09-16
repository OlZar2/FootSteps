using FluentValidation;
using FS.Contracts.Error;
using FS.Core.Enums;

namespace FS.API.RequestsModels.MissingAnnouncements.Validators;

public class DeleteMissingAnnouncementRMValidator : AbstractValidator<CancelMissingAnnouncementRM>
{
    public DeleteMissingAnnouncementRMValidator()
    {
        RuleFor(x => x.DeleteReason)
            .NotEmpty()
                .WithErrorCode(IssueCodes.Required)
            .Must(v => Enum.IsDefined(typeof(MissingAnnouncementDeleteReason), v)).WithErrorCode(IssueCodes.InvalidValue)
                .WithMessage("Недопустимое значение DeleteReason")
                .WithErrorCode(IssueCodes.InvalidValue);
    }
}