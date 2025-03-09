using Microsoft.Extensions.Configuration;
using Organizations.Domain.Common;


namespace Organizations.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    private readonly IConfiguration Configuration;

    public ApplicationDbContext(IConfiguration configuration, DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        Configuration = configuration;
    }

    public DbSet<Organization> Organizations { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Invitation> Invitations { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.Property(e => e.ProfileImage)
                .HasColumnType("bytea");

            entity.Property(e => e.BannerImage)
                .HasColumnType("bytea");
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(Configuration.GetConnectionString("OrganizationConnection"));
    }

    public override int SaveChanges()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is BaseEntity && (
                e.State == EntityState.Added
                || e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            var entity = (BaseEntity)entityEntry.Entity;

            if (entityEntry.State == EntityState.Added)
            {
                entity.DateCreated = DateTime.UtcNow;
            }
            entity.DateUpdated = DateTime.UtcNow;
        }

        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is BaseEntity && (
                e.State == EntityState.Added
                || e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            var entity = (BaseEntity)entityEntry.Entity;

            if (entityEntry.State == EntityState.Added)
            {
                entity.DateCreated = DateTime.UtcNow;
            }
            entity.DateUpdated = DateTime.UtcNow;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
