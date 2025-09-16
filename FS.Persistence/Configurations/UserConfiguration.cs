using FS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FS.Persistence.Configurations;

public class UserConfiguration: IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        
        builder.HasKey(u => u.Id);

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
        
        builder
            .HasOne(u => u.AvatarImage)
            .WithOne()
            .HasForeignKey<User>(u => u.AvatarImageId);
        
        builder.OwnsMany(u => u.Contacts, nb =>
        {
            nb.ToTable("UserContacts");
            nb.WithOwner().HasForeignKey("UserId");
            nb.HasKey("Id");
            nb.Property<Guid>("UserId");
        });
    }
}