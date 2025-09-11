using FS.Core.Enums;

namespace FS.Application.DTOs.MissingAnnouncementDTOs;

public record AnnouncementFilter
{
    public string? District { get; init; }
    public DateTime? From { get; init; }
    public PetType? Type { get; init; }
    public Gender? Gender { get; init; }
}