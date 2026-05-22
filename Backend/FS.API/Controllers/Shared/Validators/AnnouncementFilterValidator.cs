using FluentValidation;
using FS.Application.Shared.DTOs;
using FS.Contracts.Error;

namespace FS.API.Controllers.Shared.Validators;

public sealed class AnnouncementFilterValidator : AbstractValidator<AnnouncementFilter>
{
    public AnnouncementFilterValidator()
    {
        RuleFor(x => x.SearchRadius)
            .GreaterThan(0)
            .WithMessage("The search radius must be greater than 0 km.")
            .WithErrorCode(IssueCodes.TooSmall)
            .LessThanOrEqualTo(40)
            .WithMessage("The search radius cannot be more than 40 km.")
            .WithErrorCode(IssueCodes.TooLarge)
            .When(x => x.SearchRadius.HasValue);

        RuleFor(x => x.SearchCenter)
            .NotNull()
            .WithMessage("The search center point is required if a search radius is specified.")
            .WithErrorCode(IssueCodes.Required)
            .When(x => x.SearchRadius.HasValue);

        RuleFor(x => x.SearchRadius)
            .NotNull()
            .WithMessage("The search radius is required if a search center point is specified.")
            .WithErrorCode(IssueCodes.Required)
            .When(x => x.SearchCenter is not null);

        RuleFor(x => x.SearchCenter)
            .SetValidator(new CoordinatesDtoValidator()!)
            .When(x => x.SearchCenter is not null);
    }
}