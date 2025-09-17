using System.ComponentModel;
using FS.API.RequestsModels.Auth;

namespace FS.API.RequestsModels.User;

public record UpdateUserInfoRM
{
    [Description("Новый имя пользователя. Если менять не надо отправляем null. Если больше 30 символов issue TOO_LONG")]
    public string? FirstName { get; init; }
    
    [Description("Новая фамилия пользователя. Если менять не надо отправляем null. Если больше 30 символов issue TOO_LONG")]
    public string? SecondName { get; init; }
    
    [Description("Новое отчество пользователя. Если менять не надо отправляем null. Если больше 50 символов issue TOO_LONG")]
    public string? Patronymic { get; init; }
    
    [Description("Новое описание пользователя. Если менять не надо отправляем null.")]
    public string? Description { get; init; }
    
    [Description("Новые контакты пользователя. Если менять не надо отправляем null.")]
    public UserContactRM[]? UserContacts { get; init; }
}