using FluentValidation;
using FS.Contracts.Error;
using FS.Core.Enums;

namespace FS.API.RequestsModels.StreetPetAnnouncement.Validators;

public class CreateStreetPetAnnouncementRMValidator : AbstractValidator<CreateStreetPetAnnouncementRM>
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
    
    public CreateStreetPetAnnouncementRMValidator()
    {
        RuleFor(x => x.Images)
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
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(IssueCodes.Required)
            .Must(v => Enum.IsDefined(typeof(PetType), v)).WithErrorCode(IssueCodes.InvalidValue)
            .WithErrorCode(IssueCodes.InvalidValue);
        //TODO: Location может прийти null
        RuleFor(x => x.Location.Latitude)
            .NotEmpty().WithErrorCode(IssueCodes.Required);
        RuleFor(x => x.Location.Longitude)
            .NotEmpty().WithErrorCode(IssueCodes.Required);
        RuleFor(x => x.EventDate)
            .NotNull()
            .WithErrorCode(IssueCodes.Required);
    }
}