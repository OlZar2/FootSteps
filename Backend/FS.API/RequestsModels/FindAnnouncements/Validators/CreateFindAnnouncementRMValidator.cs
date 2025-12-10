using FluentValidation;
using FS.Application.Services.ImageLogic.Configurations;
using FS.Contracts.Error;
using FS.Core.AnimalAnnouncementBC.Enums;
using Microsoft.Extensions.Options;

namespace FS.API.RequestsModels.FindAnnouncements.Validators;

public class CreateFindAnnouncementRMValidator : AbstractValidator<CreateFindAnnouncementRM>
{
    public CreateFindAnnouncementRMValidator(IOptions<ImagesOptions> imagesOptions)
    {
        RuleFor(x => x.Location.Latitude)
            .NotEmpty().WithErrorCode(IssueCodes.Required);
        RuleFor(x => x.Location.Longitude)
            .NotEmpty().WithErrorCode(IssueCodes.Required);
        RuleFor(x => x.Images)
            .NotNull()
                .WithErrorCode(IssueCodes.Required)
            .NotEmpty()
                .WithErrorCode(IssueCodes.Required)
            .Must(images => images.Length is >= 1 and <= 5)
                .WithMessage("number of images must be between 1 and 5")
                .WithErrorCode(IssueCodes.TooMany);
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
        });
        RuleFor(x => x.PetType)
            .NotNull()
                .WithErrorCode(IssueCodes.Required)
            .Must(v => Enum.IsDefined(typeof(PetType), v)).WithErrorCode(IssueCodes.InvalidValue)
                .WithMessage("Недопустимое значение PetType")
                .WithErrorCode(IssueCodes.InvalidValue);
        RuleFor(x => x.Gender)
            .NotNull()
                .WithErrorCode(IssueCodes.Required)
            .Must(v => Enum.IsDefined(typeof(Gender), v)).WithErrorCode(IssueCodes.InvalidValue)
                .WithMessage("Недопустимое значение Gender")
                .WithErrorCode(IssueCodes.InvalidValue);
        RuleFor(x => x.Color)
            .MaximumLength(50).WithErrorCode(IssueCodes.TooLong);
        RuleFor(x => x.Breed)
            .MaximumLength(50).WithErrorCode(IssueCodes.TooLong);
        RuleFor(x => x.EventDate)
            .NotNull()
            .WithErrorCode(IssueCodes.Required);
    }
}