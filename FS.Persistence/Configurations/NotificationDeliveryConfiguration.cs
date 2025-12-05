using FS.Core.NotificationDomain;
using FS.Core.NotificationDomain.Entities;
using FS.Core.UserDomain.Entities;
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
            .WithMany(n => n.NotificationDeliveries)
            .HasForeignKey(i => i.NotificationId);
        
        builder.HasOne<UserDevice>()
            .WithMany()
            .HasForeignKey(i => i.UserDeviceId);
    }
}