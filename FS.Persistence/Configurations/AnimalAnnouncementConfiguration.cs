using FS.Core.Entities;
using FS.Core.Enums;
using FS.Core.ValueObjects;
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
        
        var converter = new ValueConverter<CoordinatesVO, Point>(
            coord => new Point(coord.Longitude, coord.Latitude) { SRID = 4326 },
            point => CoordinatesVO.Create(point.Y, point.X)
        );

        builder.Property(loc => loc.Location)
            .HasConversion(converter)
            .HasColumnType("geometry(Point,4326)");

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