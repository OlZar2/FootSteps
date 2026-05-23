namespace FS.Application.AuthLogic.DTOs;

public record JwtData(string Token)
{
    public string Token { get; set; } = Token;
}