using FS.Core.SearchDomain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FS.Persistence.Configurations;

public class SearchRequestImageConfiguration : IEntityTypeConfiguration<SearchRequestImage>
{
    public void Configure(EntityTypeBuilder<SearchRequestImage> builder)
    {
        builder.ToTable("SearchRequestImages");
        
        builder.HasKey(i => i.Id);
        
        builder.Property(x => x.Embedding)
            .HasColumnType("vector(512)");
    }
}