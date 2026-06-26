using FS.Application.EventLogic.Interfaces;
using FS.Core.AnimalAnnouncementBC;
using FS.Core.AnimalAnnouncementBC.Entities;
using FS.Core.ImageDomain.Entities;
using FS.Core.NotificationDomain;
using FS.Core.NotificationDomain.Entities;
using FS.Core.OutboxDomain.Entities;
using FS.Core.ReadDomain;
using FS.Core.SearchDomain;
using FS.Core.Shared.Abstractions;
using FS.Core.UserDomain;
using FS.Core.UserDomain.Entities;
using FS.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace FS.Persistence.Context;

public class ApplicationDbContext(DbContextOptions options, IDomainEventsDispatcher dispatcher) : DbContext(options)
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
        modelBuilder.ApplyConfiguration(new SimilarAnnouncementsConfiguration());
        modelBuilder.ApplyConfiguration(new SpottedLocationConfiguration());
        modelBuilder.ApplyConfiguration(new FoundReportConfiguration());
        
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.HasPostgresExtension("vector");
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        var domainEvents = ChangeTracker.Entries<AggregateRoot>()
            .SelectMany(e => e.Entity.DomainEvents)
            .ToList();

        foreach (var e in ChangeTracker.Entries<AggregateRoot>())
            e.Entity.ClearDomainEvents();
        
        var result = await base.SaveChangesAsync(ct);
        
        await dispatcher.DispatchAsync(domainEvents, ct);

        return result;
    }
    
    public DbSet<AnimalAnnouncement> AnimalAnnouncements { get; set; } = null!;
    public DbSet<PetAnnouncement> PetAnnouncements { get; set; } = null!;
    public DbSet<FindAnnouncement> FindAnnouncements { get; set; } = null!;
    public DbSet<FSImage> Images { get; set; } = null!;
    public DbSet<MissingAnnouncement> MissingAnnouncements { get; set; } = null!;
    public DbSet<StreetPetAnnouncement> StreetPetAnnouncements { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<OutboxEvent> OutboxEvents { get; set; } = null!;
    public DbSet<SearchRequest> SearchRequests { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;
    public DbSet<NotificationDelivery> NotificationDeliveries { get; set; } = null!;
    public DbSet<UserDevice> UserDevices { get; set; } = null!;
    public DbSet<SimilarAnnouncements> SimilarAnnouncements { get; set; } = null!;
    public DbSet<SpottedLocation> SpottedLocations { get; set; } = null!;
    public DbSet<FoundReport> FoundReports { get; set; } = null!;
}