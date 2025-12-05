using FS.Core.AnimalAnnouncementBC.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FS.Persistence.Configurations;

public class AnimalAnnouncementImageConfiguration: IEntityTypeConfiguration<AnimalAnnouncementImage>
{
    public void Configure(EntityTypeBuilder<AnimalAnnouncementImage> builder)
    {
        builder.ToTable("AnimalAnnouncementImages");
        
        builder.HasKey(i => i.Id);
        
        builder.Property(x => x.Embedding)
            .HasColumnType("vector(512)");
    }
}