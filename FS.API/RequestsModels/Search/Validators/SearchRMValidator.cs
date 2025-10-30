using FluentValidation;
using FS.Contracts.Error;

namespace FS.API.RequestsModels.Search.Validators;

public class SearchRMValidator : AbstractValidator<SearchRequestModel>
{
    //TODO: Вынести в конфиг
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/gif",
        "image/webp",
        "image/avif",
        "image/heic",
        "image/heif",
        "image/heic-sequence",
        "image/heif-sequence"
    };
    private const long MaxBytes = 5 * 1024 * 1024;
    
    //TODO: Ограничение на кол-во файлов
    public SearchRMValidator()
    {
        RuleFor(x => x.Image)
            .NotNull()
            .WithErrorCode(IssueCodes.Required)
            .NotEmpty()
            .WithErrorCode(IssueCodes.Required);
        RuleFor(x => x.Image.ContentType)
            .Must(ct => AllowedContentTypes.Contains(ct))
            .WithMessage("Неверный формат файла")
            .WithErrorCode(IssueCodes.InvalidFormat);
        RuleFor(x => x.Image.Length)
            .LessThanOrEqualTo(MaxBytes)
            .WithMessage("Максимальный размер файла — 5 МБ.")
            .WithErrorCode(IssueCodes.TooLarge);
    }
}