using FluentValidation;
using FS.API.Controllers.Shared.Validators;
using FS.Application.ImageLogic.Configurations;
using FS.Contracts.Error;
using FS.Core.AnimalAnnouncementBC.Enums;
using Microsoft.Extensions.Options;

namespace FS.API.Controllers.StreetPetAnnouncement.RequestModels.Validators;

public class CreateStreetPetAnnouncementRMValidator : AbstractValidator<CreateStreetPetAnnouncementRM>
{
    public CreateStreetPetAnnouncementRMValidator(ImageFileValidator imageFileValidator)
    {
        RuleFor(x => x.Images)
            .NotEmpty()
            .WithErrorCode(IssueCodes.Required);
        RuleFor(x => x.Images)
            .Must(images => images.Length is >= 1 and <= 5)
            .WithMessage("number of images must be between 1 and 5")
            .WithErrorCode(IssueCodes.TooMany)
            .When(x => x.Images != null);
        RuleForEach(x => x.Images)
            .SetValidator(imageFileValidator)
            .When(x => x.Images != null);
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