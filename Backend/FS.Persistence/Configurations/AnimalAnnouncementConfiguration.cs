using FS.Core.AnimalAnnouncementBC;
using FS.Core.AnimalAnnouncementBC.Enums;
using FS.Core.Shared.ValueObjects;
using FS.Core.UserDomain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;

namespace FS.Persistence.Configurations;

public class AnimalAnnouncementConfiguration: IEntityTypeConfiguration<AnimalAnnouncement>
{
    public void Configure(EntityTypeBuilder<AnimalAnnouncement> builder)
    {
        builder.ToTable("AnimalAnnouncements");
        
        builder.HasKey(i => i.Id);

        builder.Property(loc => loc.Location)
            .HasColumnType("geography(Point,4326)");

        builder
            .HasMany(p => p.Images)
            .WithOne();
        
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasDiscriminator(a => a.Type)
            .HasValue<FindAnnouncement>(AnnouncementType.Find)
            .HasValue<StreetPetAnnouncement>(AnnouncementType.Street)
            .HasValue<MissingAnnouncement>(AnnouncementType.Missing);
    }
}