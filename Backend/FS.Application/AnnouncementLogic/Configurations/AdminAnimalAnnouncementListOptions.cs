namespace FS.Application.AnnouncementLogic.Configurations;

public class AdminAnimalAnnouncementListOptions
{
    public int MinPageSize { get; init; } = 10;

    public int MaxPageSize { get; init; } = 30;

    public int DefaultPageSize { get; init; } = 20;
}
