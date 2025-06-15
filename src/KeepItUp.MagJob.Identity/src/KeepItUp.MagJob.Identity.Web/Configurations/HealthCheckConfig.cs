using KeepItUp.MagJob.Identity.Infrastructure.Keycloak;
using KeepItUp.MagJob.Identity.Web.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;

namespace KeepItUp.MagJob.Identity.Web.Configurations;

/// <summary>
/// Health check configuration for the application
/// </summary>
public static class HealthCheckConfig
{
    /// <summary>
    /// Adds health checks to the service collection
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration</param>
    /// <param name="logger">Logger</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddHealthCheckConfigs(
        this IServiceCollection services,
        IConfiguration configuration,
        Microsoft.Extensions.Logging.ILogger logger)
    {
        var healthChecksBuilder = services.AddHealthChecks();

        var databaseEnabled = configuration.GetValue<bool>("HealthChecks:Database:Enabled", true);
        if (databaseEnabled)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrEmpty(connectionString))
            {
                healthChecksBuilder.AddNpgSql(
                    connectionString,
                    name: "database",
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "db", "postgresql" });
            }
        }

        var keycloakEnabled = configuration.GetValue<bool>("HealthChecks:Keycloak:Enabled", true);
        if (keycloakEnabled)
        {
            var keycloakServerUrl = configuration["KeycloakAdmin:ServerUrl"];
            var keycloakRealm = configuration["KeycloakAdmin:Realm"];

            if (!string.IsNullOrEmpty(keycloakServerUrl) && !string.IsNullOrEmpty(keycloakRealm))
            {
                var keycloakTimeout = configuration.GetValue<TimeSpan>("HealthChecks:Keycloak:Timeout", TimeSpan.FromSeconds(10));

                services.AddHttpClient<KeycloakHealthCheck>(client =>
                {
                    client.Timeout = keycloakTimeout;
                });

                healthChecksBuilder.AddCheck<KeycloakHealthCheck>(
                    name: "keycloak",
                    failureStatus: HealthStatus.Degraded,
                    tags: new[] { "keycloak", "identity" });
            }
        }

        var memoryEnabled = configuration.GetValue<bool>("HealthChecks:System:Memory:Enabled", true);
        var diskEnabled = configuration.GetValue<bool>("HealthChecks:System:Disk:Enabled", true);

        if (memoryEnabled)
        {
            var maxMemoryMB = configuration.GetValue<int>("HealthChecks:System:Memory:MaxMemoryMB", 1024);
            healthChecksBuilder.AddProcessAllocatedMemoryHealthCheck(
                maximumMegabytesAllocated: maxMemoryMB,
                name: "memory",
                failureStatus: HealthStatus.Degraded,
                tags: new[] { "system", "memory" });
        }

        if (diskEnabled)
        {
            var minFreeMB = configuration.GetValue<int>("HealthChecks:System:Disk:MinFreeMB", 1024);
            var driveName = configuration.GetValue<string>("HealthChecks:System:Disk:DriveName", "");

            if (string.IsNullOrEmpty(driveName))
            {
                driveName = Environment.OSVersion.Platform == PlatformID.Win32NT ? "C:\\" : "/";
            }

            healthChecksBuilder.AddDiskStorageHealthCheck(
                options => options.AddDrive(driveName, minimumFreeMegabytes: minFreeMB),
                name: "disk",
                failureStatus: HealthStatus.Degraded,
                tags: new[] { "system", "disk" });
        }

        var enabledChecks = new List<string>();
        if (databaseEnabled) enabledChecks.Add("Database");
        if (keycloakEnabled) enabledChecks.Add("Keycloak");
        if (memoryEnabled) enabledChecks.Add("Memory");
        if (diskEnabled) enabledChecks.Add("Disk");

        logger.LogInformation("Health checks configured: {EnabledChecks}", string.Join(", ", enabledChecks));

        return services;
    }

    /// <summary>
    /// Configures health check endpoints
    /// </summary>
    /// <param name="app">Web application</param>
    /// <returns>Web application</returns>
    public static WebApplication MapHealthCheckEndpoints(this WebApplication app)
    {
        app.MapHealthChecks("/health");

        app.MapHealthChecks("/health/detailed", new HealthCheckOptions()
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
            ResultStatusCodes =
            {
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Degraded] = StatusCodes.Status200OK,
                [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
            }
        });

        app.MapHealthChecks("/health/ready", new HealthCheckOptions()
        {
            Predicate = healthCheck => healthCheck.Tags.Contains("db"),
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
            ResultStatusCodes =
            {
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Degraded] = StatusCodes.Status503ServiceUnavailable,
                [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
            }
        });

        app.MapHealthChecks("/health/live", new HealthCheckOptions()
        {
            Predicate = _ => false,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        return app;
    }
}