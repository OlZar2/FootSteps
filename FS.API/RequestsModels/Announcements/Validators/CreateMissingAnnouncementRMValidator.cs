using FluentValidation;
using FS.Contracts.Error;

namespace FS.API.RequestsModels.Announcements.Validators;

public class CreateMissingAnnouncementRMValidator : AbstractValidator<CreateMissingAnnouncementRM>
{
    //TODO: Вынести в конфиг
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/gif",
        "image/webp",
        "image/avif",
        "image/heic",
        "image/heif",
        "image/heic-sequence",
        "image/heif-sequence"
    };
    private const long MaxBytes = 5 * 1024 * 1024;
    
    //TODO: Ограничение на кол-во файлов
    public CreateMissingAnnouncementRMValidator()
    {
        RuleFor(x => x.FullPlace)
            .NotEmpty().WithErrorCode(IssueCodes.Required);
        RuleFor(x => x.District)
            .NotEmpty().WithErrorCode(IssueCodes.Required);
        RuleFor(x => x.Location.Latitude)
            .NotEmpty().WithErrorCode(IssueCodes.Required);
        RuleFor(x => x.Location.Longitude)
            .NotEmpty().WithErrorCode(IssueCodes.Required);
        RuleFor(x => x.Images)
            .NotNull()
            .WithErrorCode(IssueCodes.Required)
            .NotEmpty()
            .WithErrorCode(IssueCodes.Required);
        RuleForEach(x => x.Images).ChildRules(file =>
        {
            file.RuleFor(f => f.ContentType)
                .Must(ct => AllowedContentTypes.Contains(ct))
                .WithMessage("Неверный формат файла")
                .WithErrorCode(IssueCodes.InvalidFormat);

            file.RuleFor(f => f.Length)
                .LessThanOrEqualTo(MaxBytes)
                .WithMessage("Максимальный размер файла — 5 МБ.")
                .WithErrorCode(IssueCodes.TooLarge);
        });
        RuleFor(x => x.PetType)
            .NotNull()
                .WithErrorCode(IssueCodes.Required)
            .IsInEnum()
                .WithMessage("Недопустимое значение PetType")
                .WithErrorCode(IssueCodes.InvalidValue);
        RuleFor(x => x.Gender)
            .NotNull()
                .WithErrorCode(IssueCodes.Required)
            .IsInEnum()
                .WithMessage("Недопустимое значение Gender")
                .WithErrorCode(IssueCodes.InvalidValue);
        RuleFor(x => x.Color)
            .MaximumLength(50).WithErrorCode(IssueCodes.TooLong);
        RuleFor(x => x.Breed)
            .MaximumLength(50).WithErrorCode(IssueCodes.TooLong);
        RuleFor(x => x.PetName)
            .MaximumLength(50).WithErrorCode(IssueCodes.TooLong);
        RuleFor(x => x.EventDate)
            .NotNull()
            .WithErrorCode(IssueCodes.Required);
    }
}