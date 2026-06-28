using FS.Core.Shared.Abstractions;

namespace FS.Core.AnimalAnnouncementBC.Entities;

public class AnnouncementReport : Entity
{
    public Guid AnnouncementId { get; private set; }

    public Guid ReporterId { get; private set; }

    public string? Comment { get; private set; }

    public DateTime CreatedAt { get; private set; }

    private AnnouncementReport(
        Guid announcementId,
        Guid reporterId,
        string? comment,
        DateTime createdAt,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        AnnouncementId = announcementId;
        ReporterId = reporterId;
        Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
        CreatedAt = createdAt;
    }

    public static AnnouncementReport Create(
        Guid announcementId,
        Guid reporterId,
        string? comment = null,
        Guid? id = null)
    {
        return new AnnouncementReport(
            announcementId,
            reporterId,
            comment,
            DateTime.UtcNow,
            id);
    }

    // EF
    private AnnouncementReport() { }
}
