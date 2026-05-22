using FluentValidation;
using FS.Application.ImageLogic.Configurations;
using FS.Contracts.Error;
using FS.Core.AnimalAnnouncementBC.Enums;
using Microsoft.Extensions.Options;

namespace FS.API.Controllers.StreetPetAnnouncement.RequestModels.Validators;

public class CreateStreetPetAnnouncementRMValidator : AbstractValidator<CreateStreetPetAnnouncementRM>
{
    public CreateStreetPetAnnouncementRMValidator(IOptions<ImagesOptions> imagesOptions)
    {
        RuleFor(x => x.Images)
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