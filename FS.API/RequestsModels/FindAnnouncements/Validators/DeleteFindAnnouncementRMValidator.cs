using FluentValidation;
using FS.API.RequestsModels.MissingAnnouncements;
using FS.Contracts.Error;

namespace FS.API.RequestsModels.FindAnnouncements.Validators;

public class DeleteFindAnnouncementRMValidator : AbstractValidator<DeleteMissingAnnouncementRM>
{
    public DeleteFindAnnouncementRMValidator()
    {
        RuleFor(x => x.DeleteReason)
            .NotEmpty()
            .WithErrorCode(IssueCodes.Required)
            .IsInEnum()
            .WithMessage("Недопустимое значение DeleteReason")
            .WithErrorCode(IssueCodes.InvalidValue);
    }
}