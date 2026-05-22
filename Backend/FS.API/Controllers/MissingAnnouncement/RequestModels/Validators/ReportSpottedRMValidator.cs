using FluentValidation;
using FS.API.Controllers.Shared.Validators;
using FS.Application.ImageLogic.Configurations;
using FS.Contracts.Error;
using Microsoft.Extensions.Options;

namespace FS.API.Controllers.MissingAnnouncement.RequestModels.Validators;

public class ReportSpottedRMValidator : AbstractValidator<ReportSpottedRM>
{
    public ReportSpottedRMValidator(IOptions<ImagesOptions> imagesOptions)
    {
        RuleFor(x => x.Images)
            .NotNull();
        RuleFor(x => x.Images)
            .NotEmpty()
            .WithErrorCode(IssueCodes.Required)
            .Must(images => images.Length is >= 1 and <= 5)
            .WithMessage("number of images must be between 1 and 5")
            .WithErrorCode(IssueCodes.TooMany)
            .When(x => x.Images != null);
        RuleForEach(x => x.Images).ChildRules(file =>
        {
            file.RuleFor(f => f.ContentType)
                .Must(ct => imagesOptions.Value.AllowedContentTypes.Contains(ct))
                .WithMessage("Неверный формат файла")
                .WithErrorCode(IssueCodes.InvalidFormat);

            file.RuleFor(f => f.Length)
                .LessThanOrEqualTo(imagesOptions.Value.MaxByteSize)
                .WithMessage("Максимальный размер файла — 5 МБ.")
                .WithErrorCode(IssueCodes.TooLarge);
        })
        .When(x => x.Images != null);
        RuleFor(x => x.Coordinates)
            .NotNull()
            .WithErrorCode(IssueCodes.Required);
        RuleFor(x => x.Coordinates)
            .SetValidator(new CoordinatesDtoValidator()!)
            .When(x => x.Coordinates is not null);
    }
}