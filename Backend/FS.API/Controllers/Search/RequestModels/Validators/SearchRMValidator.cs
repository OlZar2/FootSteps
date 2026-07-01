using FluentValidation;
using FS.Contracts.Error;

namespace FS.API.Controllers.Search.RequestModels.Validators;

public class SearchRMValidator : AbstractValidator<SearchRequestModel>
{
    public SearchRMValidator()
    {
        RuleFor(x => x.Image)
            .NotNull()
            .WithErrorCode(IssueCodes.Required)
            .NotEmpty()
            .WithErrorCode(IssueCodes.Required);
        RuleFor(x => x.Location.Latitude)
            .NotEmpty().WithErrorCode(IssueCodes.Required);
        RuleFor(x => x.Location.Longitude)
            .NotEmpty().WithErrorCode(IssueCodes.Required);
    }
}