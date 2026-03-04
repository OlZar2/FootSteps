using FluentValidation;
using FS.Contracts.Error;
using Microsoft.Extensions.Options;

namespace FS.API.RequestsModels.Search.Validators;

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