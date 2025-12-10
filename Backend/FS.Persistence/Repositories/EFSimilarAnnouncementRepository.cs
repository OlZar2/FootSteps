using FS.Core.ReadDomain;
using FS.Core.ReadDomain.Stores;
using FS.Persistence.Context;

namespace FS.Persistence.Repositories;

public class EFSimilarAnnouncementRepository(ApplicationDbContext context) : ISimilarAnnouncementRepository
{
    public async Task AddRangeAsync(IEnumerable<SimilarAnnouncements> similarAnnouncements, CancellationToken ct)
    {
        context.SimilarAnnouncements.AddRange(similarAnnouncements);
        await context.SaveChangesAsync(ct);
    }
}