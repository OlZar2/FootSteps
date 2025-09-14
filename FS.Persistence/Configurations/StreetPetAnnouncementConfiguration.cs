using FS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FS.Persistence.Configurations;

public class StreetPetAnnouncementConfiguration : IEntityTypeConfiguration<StreetPetAnnouncement>
{
    public void Configure(EntityTypeBuilder<StreetPetAnnouncement> builder)
    {
        builder.Property(sp => sp.Location)
            .HasColumnType("geometry(Point,4326)");
    }
}