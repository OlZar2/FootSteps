using FS.API.RequestsModels.Auth;

namespace FS.API.RequestsModels.User;

public record UpdateUserInfoRM
{
    public string FirstName { get; init; }
    
    public string SecondName { get; init; }
    
    public string Patronymic { get; init; }
    
    public string Description { get; init; }
    
    public UserContactRM[]? UserContacts { get; init; }
}