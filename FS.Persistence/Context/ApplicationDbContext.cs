using FS.Core.Abstractions;
using FS.Core.Entities;
using FS.Notifications;
using FS.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace FS.Persistence.Context;

public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AnimalAnnouncementConfiguration());
        modelBuilder.ApplyConfiguration(new FindAnnouncementConfiguration());
        modelBuilder.ApplyConfiguration(new ImageConfiguration());
        modelBuilder.ApplyConfiguration(new MissingAnnouncementConfiguration());
        modelBuilder.ApplyConfiguration(new StreetPetAnnouncementConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new SearchRequestConfiguration());
        modelBuilder.ApplyConfiguration(new NotificationConfiguration());
        modelBuilder.ApplyConfiguration(new NotificationDeliveryConfiguration());
        modelBuilder.ApplyConfiguration(new UserDeviceConfiguration());
        
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.HasPostgresExtension("vector");
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        var result = await base.SaveChangesAsync(ct);

        var domainEvents = ChangeTracker.Entries<AggregateRoot>()
            .SelectMany(e => e.Entity.DomainEvents)
            .ToList();

        foreach (var e in ChangeTracker.Entries<AggregateRoot>())
            e.Entity.ClearDomainEvents();

        //TODO: надо UOW
        var publisher = this.GetService<IDomainEventPublisher>();
        await publisher.PublishAsync(domainEvents, ct);
        return result;
    }
    
    public DbSet<AnimalAnnouncement> AnimalAnnouncements { get; set; } = null!;
    public DbSet<PetAnnouncement> PetAnnouncements { get; set; } = null!;
    public DbSet<FindAnnouncement> FindAnnouncements { get; set; } = null!;
    public DbSet<Image> Images { get; set; } = null!;
    public DbSet<MissingAnnouncement> MissingAnnouncements { get; set; } = null!;
    public DbSet<StreetPetAnnouncement> StreetPetAnnouncements { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<OutboxEvent> OutboxEvents { get; set; } = null!;
    public DbSet<SearchRequest> SearchRequests { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;
    public DbSet<NotificationDelivery> NotificationDeliveries { get; set; } = null!;
    public DbSet<UserDevice> UserDevices { get; set; } = null!;
}