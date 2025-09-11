using FS.Core.Entities;
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
            .HasOne(pt => pt.PetType)
            .WithMany();

        builder
            .HasMany(p => p.Images)
            .WithOne();
        
        builder.OwnsOne(u => u.Place, b =>
        {
            b.Property(pn => pn.Value).HasColumnName("Place");
        });
    }
}