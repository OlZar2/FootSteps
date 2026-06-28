using FS.Core.AnimalAnnouncementBC;
using FS.Core.AnimalAnnouncementBC.Entities;
using FS.Core.UserDomain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FS.Persistence.Configurations;

public class AnnouncementReportConfiguration : IEntityTypeConfiguration<AnnouncementReport>
{
    public void Configure(EntityTypeBuilder<AnnouncementReport> builder)
    {
        builder.ToTable("AnnouncementReports");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Comment)
            .HasMaxLength(1000);

        builder.HasIndex(x => new { x.AnnouncementId, x.ReporterId })
            .IsUnique();

        builder.HasOne<AnimalAnnouncement>()
            .WithMany()
            .HasForeignKey(x => x.AnnouncementId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.ReporterId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
