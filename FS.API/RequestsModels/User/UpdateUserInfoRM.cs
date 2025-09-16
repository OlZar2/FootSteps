using System.ComponentModel;
using FS.API.RequestsModels.Auth;

namespace FS.API.RequestsModels.User;

public record UpdateUserInfoRM
{
    [Description("Новый имя пользователя. Если менять не надо отправляем null. Валидации пока нет")]
    public string? FirstName { get; init; }
    
    [Description("Новая фамилия пользователя. Если менять не надо отправляем null. Валидации пока нет")]
    public string? SecondName { get; init; }
    
    [Description("Новое отчество пользователя. Если менять не надо отправляем null. Валидации пока нет")]
    public string? Patronymic { get; init; }
    
    [Description("Новое описание пользователя. Если менять не надо отправляем null. Валидации пока нет")]
    public string? Description { get; init; }
    
    [Description("Новые контакты пользователя. Если менять не надо отправляем null. Валидации пока нет")]
    public UserContactRM[]? UserContacts { get; init; }
}