namespace FS.Application.DTOs.AuthDTOs;

public record JwtData(string Token)
{
    public string Token { get; set; } = Token;
}