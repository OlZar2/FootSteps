using System.ComponentModel;

namespace FS.API.RequestsModels.Auth;

public record RegisterRM
{
    [Description("Email пользователя. Обязательно, issue REQUIRED. Должен быть уникальным. issue: NOT_UNIQUE." +
                 " При неверном формате issue INVALID_FORMAT")]
    public string Email { get; init; }

    [Description("Пароль. Обязательный, issue REQUIRED")]
    public string Password { get; init; }

    [Description("Имя. Обязательно, issue REQUIRED. Если больше 30 символов issue TOO_LONG")]
    public string FirstName { get; init; }

    [Description("Фамилия. Обязательно, issue REQUIRED. Если больше 30 символов issue TOO_LONG")]
    public string SecondName { get; init; }

    [Description("Отчество. Опционально. Если больше 50 символов issue TOO_LONG")]
    public string? Patronymic { get; init; }

    [Description("Описание. Опционально.")]
    public string? Description { get; init; }
    
    [Description("Аватар. Обязательно, issue REQUIRED. Если расширение неверное " +
                 "issue INVALID_FORMAT или UNSUPPORTED_FORMAT или NOT_IMAGE_OR_CORRUPT " +
                 "Если больше 5МБ issue TOO_LARGE. Если файл пустой EMPTY_FILE")]
    public IFormFile? AvatarImage { get; init; }

    [Description("Контакты пользователя.")]
    public  UserContactRM[]? UserContacts { get; init; }
}