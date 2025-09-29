using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using KeepItUp.MagJob.Identity.Infrastructure.Data;
using KeepItUp.MagJob.Identity.SharedKernel.Core;

namespace KeepItUp.MagJob.Identity.IntegrationTests.Infrastructure;

/// <summary>
/// Fixture for managing PostgreSQL TestContainer for integration tests.
/// </summary>
public class DatabaseFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container;
    private readonly IDomainEventDispatcher _fakeEventDispatcher;

    public string ConnectionString => _container.GetConnectionString();

    public DatabaseFixture()
    {
        _container = new PostgreSqlBuilder()
            .WithImage("postgres:15-alpine")
            .WithDatabase("keepitup_identity_test")
            .WithUsername("test_user")
            .WithPassword("test_password")
            .WithCleanUp(true)
            .Build();

        _fakeEventDispatcher = Substitute.For<IDomainEventDispatcher>();
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        // Create and migrate database
        using var context = CreateDbContext();

        // Ensure database is clean before migrations
        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }

    public AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(ConnectionString)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
            .Options;

        return new AppDbContext(options, _fakeEventDispatcher);
    }

    public async Task ResetDatabaseAsync()
    {
        using var context = CreateDbContext();

        // Get all table names from the identity schema
        var tableNames = await context.Database.SqlQueryRaw<string>(
            "SELECT table_name FROM information_schema.tables WHERE table_schema = 'identity' AND table_type = 'BASE TABLE'")
            .ToListAsync();

        // Clear tables in reverse order to avoid FK constraints
        var tablesToClear = new[]
        {
            "Invitations",
            "MemberRoles",
            "RolePermissions",
            "Members",
            "Roles",
            "Organizations",
            "Users",
            "Permissions"
        };

        foreach (var tableName in tablesToClear)
        {
            if (tableNames.Contains(tableName))
            {
                var sql = $"TRUNCATE TABLE \"identity\".\"{tableName}\" RESTART IDENTITY CASCADE";
                await context.Database.ExecuteSqlRawAsync(sql);
            }
        }

        await context.SaveChangesAsync();
    }
}