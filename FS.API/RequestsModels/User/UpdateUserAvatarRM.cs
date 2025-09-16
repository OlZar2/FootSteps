using System.ComponentModel;

namespace FS.API.RequestsModels.User;

//TODO: Сделать общую валидацию через config
public class UpdateUserAvatarRM
{
    [Description("Новый аватар пользователя. Если отправить пустой, то аватар удалится. Валидации пока нет")]
    public IFormFile? AvatarImage { get; init; }
}