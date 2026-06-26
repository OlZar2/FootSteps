namespace FS.Application.AuthLogic.Configurations;

public class EmailConfirmationOptions
{
    public string ConfirmationUrlTemplate { get; set; } =
        "http://localhost:5000/api/auth/confirm-email?userId={userId}&token={token}";
}
