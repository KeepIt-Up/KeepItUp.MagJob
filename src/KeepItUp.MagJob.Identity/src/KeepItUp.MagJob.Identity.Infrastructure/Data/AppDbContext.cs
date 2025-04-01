using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.Infrastructure.Data.Config;
using KeepItUp.MagJob.Identity.SharedKernel;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace KeepItUp.MagJob.Identity.Infrastructure.Data;
public class AppDbContext(DbContextOptions<AppDbContext> options,
  IDomainEventDispatcher? dispatcher) : DbContext(options)
{
    private readonly IDomainEventDispatcher? _dispatcher = dispatcher;

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
        // Dodanie rozszerzenia dla UUID
        modelBuilder.HasPostgresExtension("uuid-ossp");
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        UpdateTimestamps();

        int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        if (_dispatcher == null) return result;

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

    /// <summary>
    /// Czyści śledzenie encji o określonym ID.
    /// </summary>
    /// <typeparam name="TEntity">Typ encji.</typeparam>
    /// <param name="id">ID encji do usunięcia ze śledzenia.</param>
    public void DetachEntityById<TEntity>(Guid id) where TEntity : class
    {
        var entry = ChangeTracker.Entries<TEntity>()
            .FirstOrDefault(e => EF.Property<Guid>(e.Entity, "Id").Equals(id));

        if (entry != null)
        {
            entry.State = EntityState.Detached;
        }
    }

    /// <summary>
    /// Czyści kontekst z wszystkich śledzonych encji.
    /// </summary>
    public void ClearChangeTracker()
    {
        ChangeTracker.Clear();
    }
}
