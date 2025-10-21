using FS.Core.Entities;
using FS.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FS.Persistence.Configurations;

public class AnimalAnnouncementConfiguration: IEntityTypeConfiguration<AnimalAnnouncement>
{
    public void Configure(EntityTypeBuilder<AnimalAnnouncement> builder)
    {
        builder.ToTable("AnimalAnnouncements");
        
        builder.HasKey(i => i.Id);

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