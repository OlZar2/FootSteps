using FS.Core.AnimalAnnouncementBC;
using FS.Core.ReadDomain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FS.Persistence.Configurations;

public class SimilarAnnouncementsConfiguration : IEntityTypeConfiguration<SimilarAnnouncements>
{
    public void Configure(EntityTypeBuilder<SimilarAnnouncements> builder)
    {
        builder.HasKey(x => new { x.StreetPetAnnouncementId, x.MissingAnnouncementId });
        
        builder.HasOne<StreetPetAnnouncement>()
            .WithMany()
            .HasForeignKey(a => a.StreetPetAnnouncementId);
        
        builder.HasOne<MissingAnnouncement>()
            .WithMany()
            .HasForeignKey(a => a.MissingAnnouncementId);
    }
}