namespace FS.Application.AnnouncementLogic.DTOs;

public class AdminAnimalAnnouncementListQuery
{
    public int? PageSize { get; set; }

    public string? Cursor { get; set; }

    public DateTime? CreatedAtFrom { get; set; }

    public DateTime? CreatedAtTo { get; set; }

    public int? ReportsCountFrom { get; set; }

    public int? ReportsCountTo { get; set; }

    public AdminAnimalAnnouncementSortBy SortBy { get; set; } = AdminAnimalAnnouncementSortBy.CreatedAt;

    public SortDirection SortDirection { get; set; } = SortDirection.Desc;
}
