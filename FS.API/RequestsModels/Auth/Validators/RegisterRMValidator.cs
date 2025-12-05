using FluentValidation;
using FS.Application.Services.ImageLogic.Configurations;
using FS.Contracts.Error;
using Microsoft.Extensions.Options;

namespace FS.API.RequestsModels.Auth.Validators;

public class RegisterRMValidator : AbstractValidator<RegisterRM>
{
    public RegisterRMValidator(IOptions<ImagesOptions> imagesOptions)
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
            .MaximumLength(40).WithErrorCode(IssueCodes.TooLong);
        RuleFor(x => x.Patronymic)
            .MaximumLength(50).WithErrorCode(IssueCodes.TooLong);
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
        RuleForEach(x => x.UserContacts)
            .SetValidator(new UserContactRMValidator());
    }
}