using FS.Application.Shared.DTOs;

namespace FS.Application.SearchLogic.DTOs;

public record SearchRequestDto
{
    public required Guid UserId { get; init; }
    public required FileData Image { get; init; }

    public required CoordinatesDto Location { get; init; }
}