using FS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FS.Persistence.Configurations;

public class NotificationDeliveryConfiguration : IEntityTypeConfiguration<NotificationDelivery>
{
    public void Configure(EntityTypeBuilder<NotificationDelivery> builder)
    {
        builder.ToTable("NotificationDeliveries");
        
        builder.HasKey(i => i.Id);

        builder.HasOne<Notification>()
            .WithMany()
            .HasForeignKey(i => i.NotificationId);
        
        builder.HasOne(nd => nd.User)
            .WithMany()
            .HasForeignKey(i => i.UserId);
    }
}