namespace FS.Application.AnnouncementLogic.DTOs;

public class AdminAnimalAnnouncementListPage
{
    public AdminAnimalAnnouncementListItem[] Items { get; init; } = [];

    public string? NextCursor { get; init; }
}
