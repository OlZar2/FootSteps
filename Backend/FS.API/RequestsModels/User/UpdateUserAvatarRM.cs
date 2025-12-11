using System.ComponentModel;

namespace FS.API.RequestsModels.User;

public class UpdateUserAvatarRM
{
    [Description("Guid Нового аватара пользователя. Если отправить пустой, то аватар удалится")]
    public Guid? AvatarImageId { get; init; }
}