using FS.Core.AnimalAnnouncementBC.Enums;

namespace FS.Application.AnnouncementLogic.DTOs;

public class AdminAnimalAnnouncementListItem
{
    public Guid Id { get; init; }

    public string[] ImagePaths { get; init; } = [];

    public string? Description { get; init; }

    public int ReportsCount { get; init; }

    public DateTime CreatedAt { get; init; }

    public AnnouncementType Type { get; init; }
}
