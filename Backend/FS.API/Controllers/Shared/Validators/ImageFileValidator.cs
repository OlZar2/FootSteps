using FluentValidation;
using FS.Application.ImageLogic.Configurations;
using FS.Contracts.Error;
using Microsoft.Extensions.Options;

namespace FS.API.Controllers.Shared.Validators;

public class ImageFileValidator : AbstractValidator<IFormFile>
{
    public ImageFileValidator(IOptions<ImagesOptions> imagesOptions)
    {
        var options = imagesOptions.Value;

        RuleFor(x => x)
            .NotNull()
            .WithErrorCode(IssueCodes.Required);

        RuleFor(x => x.Length)
            .GreaterThan(0)
            .WithErrorCode(IssueCodes.Required);

        RuleFor(x => x.ContentType)
            .Must(contentType => options.AllowedContentTypes.Contains(contentType))
            .WithMessage("Неверный формат файла")
            .WithErrorCode(IssueCodes.InvalidFormat);

        RuleFor(x => x.Length)
            .LessThanOrEqualTo(options.MaxByteSize)
            .WithMessage("Максимальный размер файла — 5 МБ.")
            .WithErrorCode(IssueCodes.TooLarge);
    }
}