using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using KeepItUp.MagJob.Identity.Infrastructure.Data;

namespace KeepItUp.MagJob.Identity.IntegrationTests.Infrastructure;

/// <summary>
/// Custom WebApplicationFactory for integration tests with TestContainer database.
/// </summary>
public class TestWebApplicationFactory : WebApplicationFactory<Program>, IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _databaseFixture;

    public TestWebApplicationFactory(DatabaseFixture databaseFixture)
    {
        _databaseFixture = databaseFixture;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add DbContext with TestContainer connection string
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(_databaseFixture.ConnectionString);
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            });

            // Remove Keycloak services for tests
            var keycloakServices = services.Where(s =>
                s.ServiceType.FullName?.Contains("Keycloak") == true ||
                s.ImplementationType?.FullName?.Contains("Keycloak") == true)
                .ToList();

            foreach (var service in keycloakServices)
            {
                services.Remove(service);
            }

            // Add mock Keycloak services
            services.AddSingleton<Core.Interfaces.IKeycloakClient, MockKeycloakClient>();
            services.AddSingleton<Core.Interfaces.IKeycloakSyncService, MockKeycloakSyncService>();

            // Database is already migrated in DatabaseFixture
        });

        builder.UseEnvironment("Testing");

        // Override Keycloak configuration for tests
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["KeycloakAdmin:ServerUrl"] = "http://localhost:8080",
                ["KeycloakAdmin:Realm"] = "test-realm",
                ["KeycloakAdmin:ClientId"] = "test-client",
                ["KeycloakAdmin:ClientSecret"] = "test-secret",
                ["KeycloakAdmin:RequireHttps"] = "false",
                ["KeycloakAdmin:AdminUsername"] = "admin",
                ["KeycloakAdmin:AdminPassword"] = "admin",
                ["KeycloakClient:ServerUrl"] = "http://localhost:8080",
                ["KeycloakClient:Realm"] = "test-realm",
                ["KeycloakClient:ClientId"] = "test-client",
                ["HealthChecks:Keycloak:Enabled"] = "false",
                ["Database:SkipMigrations"] = "true"
            });
        });

        // Suppress logging noise in tests
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Warning);
        });
    }

    public async Task ResetDatabaseAsync()
    {
        await _databaseFixture.ResetDatabaseAsync();
    }

    public AppDbContext CreateDbContext()
    {
        return _databaseFixture.CreateDbContext();
    }
}