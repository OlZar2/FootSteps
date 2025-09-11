using FS.Core.Entities;
using FS.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace FS.Persistence.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AnnouncementConfiguration());
        modelBuilder.ApplyConfiguration(new FindAnnouncementConfiguration());
        modelBuilder.ApplyConfiguration(new ImageConfiguration());
        modelBuilder.ApplyConfiguration(new MissingAnnouncementConfiguration());
        modelBuilder.ApplyConfiguration(new PetTypeConfiguration());
        modelBuilder.ApplyConfiguration(new StreetPetAnnouncementConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());

        base.OnModelCreating(modelBuilder);
    }
    
    public DbSet<Announcement> Announcements { get; set; } = null!;
    public DbSet<FindAnnouncement> FindAnnouncements { get; set; } = null!;
    public DbSet<Image> Images { get; set; } = null!;
    public DbSet<MissingAnnouncement> MissingAnnouncements { get; set; } = null!;
    public DbSet<PetType> PetTypes { get; set; } = null!;
    public DbSet<StreetPetAnnouncement> StreetPetAnnouncements { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
}