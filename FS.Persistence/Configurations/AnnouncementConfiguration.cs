using FS.Core.Entities;
using FS.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FS.Persistence.Configurations;

public class AnnouncementConfiguration: IEntityTypeConfiguration<Announcement>
{
    public void Configure(EntityTypeBuilder<Announcement> builder)
    {
        builder.ToTable("Announcements");
        
        builder.HasKey(i => i.Id);

        builder
            .HasMany(p => p.Images)
            .WithOne();
        
        builder.OwnsOne(u => u.FullPlace, b =>
        {
            b.Property(pn => pn.Value).HasColumnName("FullPlace");
        });
        
        builder.OwnsOne(u => u.District, b =>
        {
            b.Property(pn => pn.Value).HasColumnName("District");
        });
        
        builder
            .HasDiscriminator(a => a.Type)
            .HasValue<FindAnnouncement>(AnnouncementType.Find)
            .HasValue<StreetPetAnnouncement>(AnnouncementType.Street)
            .HasValue<MissingAnnouncement>(AnnouncementType.Missing);
    }
}