namespace FS.Core.ReadDomain.Stores;

public interface ISimilarAnnouncementRepository
{
    Task AddRangeAsync(IEnumerable<SimilarAnnouncements> similarAnnouncements, CancellationToken ct);
}