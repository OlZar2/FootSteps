using FluentValidation;
using FS.Application.Shared.DTOs;

namespace FS.API.Controllers.Shared.Validators;

public sealed class CoordinatesDtoValidator : AbstractValidator<CoordinatesDto>
{
    public CoordinatesDtoValidator()
    {
        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90)
            .WithMessage("Latitude must be between -90 and 90.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180)
            .WithMessage("Longitude should be between -180 and 180.");
    }
}