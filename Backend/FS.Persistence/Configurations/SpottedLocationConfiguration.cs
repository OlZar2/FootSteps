using FS.Core.AnimalAnnouncementBC.Entities;
using FS.Core.Shared.ValueObjects;
using FS.Core.UserDomain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;

namespace FS.Persistence.Configurations;

public class SpottedLocationConfiguration : IEntityTypeConfiguration<SpottedLocation>
{
    public void Configure(EntityTypeBuilder<SpottedLocation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(sl => sl.SpottedUserId);
        
        var converter = new ValueConverter<CoordinatesVO, Point>(
            coord => new Point(coord.Longitude, coord.Latitude) { SRID = 4326 },
            point => CoordinatesVO.Create(point.Y, point.X)
        );
        
        builder.Property(loc => loc.Location)
            .HasConversion(converter)
            .HasColumnType("geometry(Point,4326)");
        
        builder.Property(sp => sp.Location)
            .HasColumnType("geometry(Point,4326)");
    }
}