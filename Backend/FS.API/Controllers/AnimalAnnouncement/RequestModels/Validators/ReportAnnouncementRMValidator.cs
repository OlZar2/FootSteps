using FluentValidation;
using FS.Contracts.Error;

namespace FS.API.Controllers.AnimalAnnouncement.RequestModels.Validators;

public class ReportAnnouncementRMValidator : AbstractValidator<ReportAnnouncementRM>
{
    public ReportAnnouncementRMValidator()
    {
        RuleFor(x => x.Comment)
            .MaximumLength(1000)
            .WithErrorCode(IssueCodes.TooLong);
    }
}
