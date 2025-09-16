using FluentValidation;
using FS.Contracts.Error;
using FS.Core.Enums;

namespace FS.API.RequestsModels.FindAnnouncements.Validators;

public class DeleteFindAnnouncementRMValidator : AbstractValidator<DeleteFindAnnouncementRM>
{
    public DeleteFindAnnouncementRMValidator()
    {
        RuleFor(x => x.DeleteReason)
            .NotEmpty()
            .WithErrorCode(IssueCodes.Required)
            .Must(v => Enum.IsDefined(typeof(FindAnnouncementDeleteReason), v)).WithErrorCode(IssueCodes.InvalidValue)
            .WithMessage("Недопустимое значение DeleteReason")
            .WithErrorCode(IssueCodes.InvalidValue);
    }
}