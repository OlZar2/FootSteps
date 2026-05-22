using FluentValidation;
using FS.Contracts.Error;

namespace FS.API.Controllers.Search.RequestModels.Validators;

public class SearchRMValidator : AbstractValidator<SearchRequestModel>
{
    public SearchRMValidator()
    {
        RuleFor(x => x.ImageId)
            .NotNull()
            .WithErrorCode(IssueCodes.Required)
            .NotEmpty()
            .WithErrorCode(IssueCodes.Required);
    }
}