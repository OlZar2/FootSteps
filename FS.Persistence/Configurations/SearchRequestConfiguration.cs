using FS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FS.Persistence.Configurations;

public class SearchRequestConfiguration: IEntityTypeConfiguration<SearchRequest>
{
    public void Configure(EntityTypeBuilder<SearchRequest> builder)
    {
        builder.ToTable("SearchRequests");
        
        builder.HasKey(i => i.Id);
        
        builder.Property(x => x.Embedding)
            .HasColumnType("vector(512)");
        
        builder
            .HasMany(r => r.Results)
            .WithMany()
            .UsingEntity(
                "SearchResults",
                r => r.HasOne(typeof(AnimalAnnouncement)).WithMany().HasForeignKey("AnimalAnnouncementId").OnDelete(DeleteBehavior.NoAction),
                l => l.HasOne(typeof(SearchRequest)).WithMany().HasForeignKey("SearchRequestId").OnDelete(DeleteBehavior.NoAction)
            );
    }
}