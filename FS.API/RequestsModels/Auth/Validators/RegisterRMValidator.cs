using FluentValidation;
using FS.Contracts.Error;

namespace FS.API.RequestsModels.Auth.Validators;

public class RegisterRMValidator : AbstractValidator<RegisterRM>
{
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
    
    public RegisterRMValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithErrorCode(IssueCodes.Required)
            .EmailAddress().WithErrorCode(IssueCodes.InvalidFormat);
        RuleFor(x => x.Password)
            .NotEmpty().WithErrorCode(IssueCodes.Required);
        RuleFor(x => x.FirstName)
            .NotEmpty().WithErrorCode(IssueCodes.Required)
            .MaximumLength(30).WithErrorCode(IssueCodes.TooLong);
        RuleFor(x => x.SecondName)
            .NotEmpty().WithErrorCode(IssueCodes.Required)
            .MaximumLength(40).WithErrorCode(IssueCodes.TooLong);
        RuleFor(x => x.Patronymic)
            .MaximumLength(50).WithErrorCode(IssueCodes.TooLong);
        RuleFor(x => x.AvatarImage)
            .NotNull().WithErrorCode(IssueCodes.Required)
            .DependentRules(() =>
            {
                When(x => x.AvatarImage is not null, () =>
                {
                    RuleFor(x => x.AvatarImage.ContentType)
                        .Must(ct => AllowedContentTypes.Contains(ct))
                        .WithMessage("Неверный формат файла")
                        .WithErrorCode(IssueCodes.InvalidFormat);

                    RuleFor(x => x.AvatarImage.Length)
                        .LessThanOrEqualTo(MaxBytes)
                        .WithMessage("Максимальный размер файла — 5 МБ.")
                        .WithErrorCode(IssueCodes.TooLarge);
                });
            });
}
}