using KeepItUp.MagJob.Identity.Core.ContributorAggregate;
using KeepItUp.MagJob.Identity.Core.SharedKernel;
using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Infrastructure.Data.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection;
using Ardalis.SharedKernel;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace KeepItUp.MagJob.Identity.Infrastructure.Data;
public class AppDbContext(DbContextOptions<AppDbContext> options,
  IDomainEventDispatcher? dispatcher) : DbContext(options)
{
  private readonly IDomainEventDispatcher? _dispatcher = dispatcher;

  public DbSet<Contributor> Contributors => Set<Contributor>();
  public DbSet<User> Users => Set<User>();
  public DbSet<Organization> Organizations => Set<Organization>();
  public DbSet<Member> Members => Set<Member>();
  public DbSet<Role> Roles => Set<Role>();
  public DbSet<Invitation> Invitations => Set<Invitation>();
  public DbSet<Permission> Permissions => Set<Permission>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    // Zastosowanie konfiguracji z assembly
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

    // Utworzenie schematu "identity"
    modelBuilder.HasDefaultSchema(DataSchemaConstants.IDENTITY_SCHEMA);
    modelBuilder.HasPostgresExtension("uuid-ossp"); // Dodanie rozszerzenia dla UUID
  }

  public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
  {
    UpdateTimestamps();
    
    int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

    // ignore events if no dispatcher provided
    if (_dispatcher == null) return result;

    // dispatch events only if save was successful
    var entitiesWithEvents = ChangeTracker.Entries<EntityBase>()
        .Select(e => e.Entity)
        .Where(e => e.DomainEvents.Any())
        .ToArray();

    await _dispatcher.DispatchAndClearEvents(entitiesWithEvents);

    return result;
  }

  public override int SaveChanges()
  {
    UpdateTimestamps();
    return base.SaveChanges();
  }
  
  /// <summary>
  /// Automatycznie aktualizuje pola CreatedAt i UpdatedAt dla encji dziedziczących z BaseEntity.
  /// </summary>
  private void UpdateTimestamps()
  {
    var entities = ChangeTracker.Entries()
        .Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

    var now = DateTime.UtcNow;

    foreach (var entity in entities)
    {
        var baseEntity = (BaseEntity)entity.Entity;
        
        if (entity.State == EntityState.Added)
        {
            entity.Property("CreatedAt").CurrentValue = now;
        }

        entity.Property("UpdatedAt").CurrentValue = now;
    }
  }
}
