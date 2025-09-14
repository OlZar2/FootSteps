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
            .Custom((file, context) =>
            {
                if (file is null) return;

                if (!AllowedContentTypes.Contains(file.ContentType))
                {
                    context.AddFailure(new FluentValidation.Results.ValidationFailure(
                        nameof(context.InstanceToValidate.AvatarImage),
                        "Неверный формат файла")
                    {
                        ErrorCode = IssueCodes.InvalidFormat
                    });
                }
                
                if (file.Length > MaxBytes)
                {
                    context.AddFailure(new FluentValidation.Results.ValidationFailure(
                        nameof(context.InstanceToValidate.AvatarImage),
                        "Максимальный размер файла — 5 МБ.")
                    {
                        ErrorCode = IssueCodes.TooLarge
                    });
                }
            });
        RuleForEach(x => x.UserContacts)
            .SetValidator(new UserContactRMValidator());
    }
}