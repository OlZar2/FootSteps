using FS.Core.ImageDomain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FS.Persistence.Configurations;

public class ImageConfiguration: IEntityTypeConfiguration<FSImage>
{
    public void Configure(EntityTypeBuilder<FSImage> builder)
    {
        builder.ToTable("Images");
        
        builder.HasKey(i => i.Id);
        
        builder.Property(x => x.Embedding)
            .HasColumnType("vector(512)");
    }
}