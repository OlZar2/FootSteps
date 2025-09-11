using FS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FS.Persistence.Configurations;

public class FindAnnouncementConfiguration: IEntityTypeConfiguration<FindAnnouncement>
{
    public void Configure(EntityTypeBuilder<FindAnnouncement> builder)
    {
    }
}