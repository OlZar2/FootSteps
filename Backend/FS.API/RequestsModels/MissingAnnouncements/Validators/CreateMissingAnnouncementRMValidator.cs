using FluentValidation;
using FS.Application.Services.ImageLogic.Configurations;
using FS.Contracts.Error;
using FS.Core.AnimalAnnouncementBC.Enums;
using Microsoft.Extensions.Options;

namespace FS.API.RequestsModels.MissingAnnouncements.Validators;

public class CreateMissingAnnouncementRMValidator : AbstractValidator<CreateMissingAnnouncementRM>
{
    public CreateMissingAnnouncementRMValidator()
    {
        RuleFor(x => x.Location.Latitude)
            .NotEmpty().WithErrorCode(IssueCodes.Required);
        RuleFor(x => x.Location.Longitude)
            .NotEmpty().WithErrorCode(IssueCodes.Required);
        RuleFor(x => x.ImageIds)
            .NotNull()
                .WithErrorCode(IssueCodes.Required)
            .NotEmpty()
                .WithErrorCode(IssueCodes.Required)
            .Must(images => images.Length is >= 1 and <= 5)
                .WithMessage("number of images must be between 1 and 5")
                .WithErrorCode(IssueCodes.TooMany);
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
        RuleFor(x => x.PetName)
            .MaximumLength(50).WithErrorCode(IssueCodes.TooLong);
        RuleFor(x => x.EventDate)
            .NotNull()
            .WithErrorCode(IssueCodes.Required);
    }
}