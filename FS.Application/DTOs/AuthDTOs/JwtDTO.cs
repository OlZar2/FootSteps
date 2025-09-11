namespace FS.Application.DTOs.AuthDTOs;

public record JwtDTO(string Token)
{
    public string Token { get; set; } = Token;
}