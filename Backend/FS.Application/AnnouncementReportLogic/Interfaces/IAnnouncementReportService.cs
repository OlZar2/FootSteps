using FS.Application.AnnouncementReportLogic.DTOs;

namespace FS.Application.AnnouncementReportLogic.Interfaces;

public interface IAnnouncementReportService
{
    Task ReportAsync(ReportAnnouncementData data, CancellationToken ct);
}
