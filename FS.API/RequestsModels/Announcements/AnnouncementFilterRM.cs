using FS.Core.Enums;

namespace FS.API.RequestsModels.Announcements;

public class AnnouncementFilterRM
{
    public string? District { get; init; }
    public DateTime? From { get; init; }
    public PetType? Type { get; init; }
    public Gender? Gender { get; init; }
}