using FluentValidation;
using FS.Contracts.Error;

namespace FS.API.RequestsModels.User.Validators;

public class UpdateUserAvatarRMValidator : AbstractValidator<UpdateUserAvatarRM>
{
    //TODO: в конфиг
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
    
    public UpdateUserAvatarRMValidator()
    {
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
    }
}