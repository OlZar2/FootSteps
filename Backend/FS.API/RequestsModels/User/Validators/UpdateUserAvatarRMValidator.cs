using FluentValidation;
using FS.Application.Services.ImageLogic.Configurations;
using FS.Contracts.Error;
using Microsoft.Extensions.Options;

namespace FS.API.RequestsModels.User.Validators;

public class UpdateUserAvatarRMValidator : AbstractValidator<UpdateUserAvatarRM>
{
    public UpdateUserAvatarRMValidator(IOptions<ImagesOptions> imagesOptions)
    {
        RuleFor(x => x.AvatarImage)
            .Custom((file, context) =>
            {
                if (file is null) return;

                if (!imagesOptions.Value.AllowedContentTypes.Contains(file.ContentType))
                {
                    context.AddFailure(new FluentValidation.Results.ValidationFailure(
                        nameof(context.InstanceToValidate.AvatarImage),
                        "Неверный формат файла")
                    {
                        ErrorCode = IssueCodes.InvalidFormat
                    });
                }
                
                if (file.Length > imagesOptions.Value.MaxByteSize)
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