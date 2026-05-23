using FluentValidation;
using FS.Contracts.Error;
using FS.Core.AnimalAnnouncementBC.Enums;

namespace FS.API.Controllers.FindAnnouncement.RequestModels.Validators;

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