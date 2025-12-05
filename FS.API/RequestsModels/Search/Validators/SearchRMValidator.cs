using FluentValidation;
using FS.Application.Services.ImageLogic.Configurations;
using FS.Contracts.Error;
using Microsoft.Extensions.Options;

namespace FS.API.RequestsModels.Search.Validators;

public class SearchRMValidator : AbstractValidator<SearchRequestModel>
{
    public SearchRMValidator(IOptions<ImagesOptions> imagesOptions)
    {
        RuleFor(x => x.Image)
            .NotNull()
            .WithErrorCode(IssueCodes.Required)
            .NotEmpty()
            .WithErrorCode(IssueCodes.Required);
        RuleFor(x => x.Image.ContentType)
            .Must(ct => imagesOptions.Value.AllowedContentTypes.Contains(ct))
            .WithMessage("Неверный формат файла")
            .WithErrorCode(IssueCodes.InvalidFormat);
        RuleFor(x => x.Image.Length)
            .LessThanOrEqualTo(imagesOptions.Value.MaxByteSize)
            .WithMessage("Максимальный размер файла — 5 МБ.")
            .WithErrorCode(IssueCodes.TooLarge);
    }
}