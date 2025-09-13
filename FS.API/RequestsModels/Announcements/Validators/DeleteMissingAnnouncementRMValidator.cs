using FluentValidation;
using FS.Contracts.Error;

namespace FS.API.RequestsModels.Announcements.Validators;

public class DeleteMissingAnnouncementRMValidator : AbstractValidator<DeleteMissingAnnouncementRM>
{
    public DeleteMissingAnnouncementRMValidator()
    {
        RuleFor(x => x.DeleteReason)
            .NotEmpty()
                .WithErrorCode(IssueCodes.Required)
            .IsInEnum()
                .WithMessage("Недопустимое значение DeleteReason")
                .WithErrorCode(IssueCodes.InvalidValue);
    }
}