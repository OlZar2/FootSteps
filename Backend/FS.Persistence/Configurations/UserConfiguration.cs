using FS.Core.UserDomain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FS.Persistence.Configurations;

public class UserConfiguration: IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        
        builder.HasKey(u => u.Id);

        builder.HasMany(u => u.UserDevices)
            .WithOne()
            .HasForeignKey(ud => ud.UserId);
        
        builder.OwnsOne(u => u.Email, b =>
        {
            b.Property(pn => pn.Value).HasColumnName("Email");
        });

        builder.OwnsOne(u => u.FullName, b =>
        {
            b.Property(fn => fn.FirstName).HasColumnName("FirstName");
            b.Property(fn => fn.SecondName).HasColumnName("SecondName");
            b.Property(fn => fn.Patronymic).HasColumnName("Patronymic");
        });
        
        builder.Property(loc => loc.LastCoordinates)
            .HasColumnType("geography(Point,4326)");
        
        builder
            .HasOne(u => u.AvatarImage)
            .WithOne()
            .HasForeignKey<User>(u => u.AvatarImageId)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.OwnsMany(u => u.Contacts, nb =>
        {
            nb.ToTable("UserContacts");
            nb.WithOwner().HasForeignKey("UserId");
            nb.HasKey("Id");
            nb.Property<Guid>("UserId");
            nb.Property(c => c.Id).ValueGeneratedNever();
        });
    }
}