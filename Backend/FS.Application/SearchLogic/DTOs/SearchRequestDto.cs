namespace FS.Application.SearchLogic.DTOs;

public record SearchRequestDto
{
    public required Guid UserId { get; init; }
    public required Guid ImageId { get; init; }
}