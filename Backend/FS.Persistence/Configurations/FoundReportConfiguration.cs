using FS.Core.AnimalAnnouncementBC.Entities;
using FS.Core.UserDomain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FS.Persistence.Configurations;

public class FoundReportConfiguration : IEntityTypeConfiguration<FoundReport>
{
    public void Configure(EntityTypeBuilder<FoundReport> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(sl => sl.FoundUserId);

        builder.HasMany(sl => sl.Images)
            .WithOne();
    }
}