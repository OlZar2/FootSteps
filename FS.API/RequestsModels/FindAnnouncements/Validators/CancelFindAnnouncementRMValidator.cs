using FluentValidation;
using FS.Contracts.Error;
using FS.Core.Enums;

namespace FS.API.RequestsModels.FindAnnouncements.Validators;

public class CancelFindAnnouncementRMValidator : AbstractValidator<CancelFindAnnouncementRM>
{
    public CancelFindAnnouncementRMValidator()
    {
        RuleFor(x => x.CancelReason)
            .NotEmpty()
            .WithErrorCode(IssueCodes.Required)
            .Must(v => Enum.IsDefined(typeof(FindAnnouncementDeleteReason), v)).WithErrorCode(IssueCodes.InvalidValue)
            .WithMessage("Недопустимое значение DeleteReason")
            .WithErrorCode(IssueCodes.InvalidValue);
    }
}