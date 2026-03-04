using FS.Application.DTOs.Shared;

namespace FS.Application.DTOs.UserDTOs;

public record UpdateUserAvatar
{
    public required Guid UserId { get; init; }
    
    public required FileData? Avatar { get; init; }
}