using System.ComponentModel;

namespace FS.API.RequestsModels.User;

//TODO: Сделать общую валидацию через config
public class UpdateUserAvatarRM
{
    [Description("Новый аватар пользователя. Если отправить пустой, то аватар удалится. Если расширение неверное " +
                 "issue INVALID_FORMAT или UNSUPPORTED_FORMAT или NOT_IMAGE_OR_CORRUPT " +
                 "Если больше 5МБ issue TOO_LARGE. Если файл пустой EMPTY_FILE")]
    public IFormFile? AvatarImage { get; init; }
}