using FS.Application.Shared.DTOs;

namespace FS.Application.UserLogic.DTOs;

public record UpdateUserAvatar
{
    public required Guid UserId { get; init; }
    
    public required FileData? Avatar { get; init; }
}