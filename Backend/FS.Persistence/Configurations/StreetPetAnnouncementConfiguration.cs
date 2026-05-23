using FS.Core.AnimalAnnouncementBC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FS.Persistence.Configurations;

public class StreetPetAnnouncementConfiguration : IEntityTypeConfiguration<StreetPetAnnouncement>
{
    public void Configure(EntityTypeBuilder<StreetPetAnnouncement> builder)
    {
    }
}