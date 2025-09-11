using FS.Core.Entities;
using FS.Core.Specifications;

namespace FS.Core.Stores;

public interface IMissingAnnouncementRepository
{
    Task<MissingAnnouncement[]> GetFilteredMissingAnnouncementByPageAsync(DateTime lastDateTime,
        MissingAnnouncementSpecification spec);
}