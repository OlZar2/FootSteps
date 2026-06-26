using FS.Core.AnimalAnnouncementBC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FS.Persistence.Configurations;

public class MissingAnnouncementConfiguration: IEntityTypeConfiguration<MissingAnnouncement>
{
    public void Configure(EntityTypeBuilder<MissingAnnouncement> builder)
    {
        builder.HasMany(ma => ma.SpottedLocations)
            .WithOne()
            .HasForeignKey(sl => sl.MissingAnnouncementId);
        
        builder.HasMany(ma => ma.FoundReports)
            .WithOne()
            .HasForeignKey(fr => fr.MissingAnnouncementId);
    }
}