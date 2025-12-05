using FS.Core.AnimalAnnouncementBC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FS.Persistence.Configurations;

public class MissingAnnouncementConfiguration: IEntityTypeConfiguration<MissingAnnouncement>
{
    public void Configure(EntityTypeBuilder<MissingAnnouncement> builder)
    {
    }
}