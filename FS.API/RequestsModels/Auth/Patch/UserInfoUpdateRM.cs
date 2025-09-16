namespace FS.API.RequestsModels.Auth.Patch;

public class UserInfoUpdateRM
{
    public string FirstName { get; set; }
    public string SecondName { get; set; }
    public string Patrinymic { get; set; }
    public string Description { get; set; }
    public UserContactRM[]? UserContacts { get; init; }
}