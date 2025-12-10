namespace FS.Application.DTOs.SearchDTOs;

public record SearchRequestDto
{
    public required Guid UserId { get; init; }
    public required byte[] Image { get; init; }
}