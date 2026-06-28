namespace FS.Application.AnnouncementReportLogic.DTOs;

public class ReportAnnouncementData
{
    public Guid AnnouncementId { get; init; }

    public Guid ReporterId { get; init; }

    public string? Comment { get; init; }
}
