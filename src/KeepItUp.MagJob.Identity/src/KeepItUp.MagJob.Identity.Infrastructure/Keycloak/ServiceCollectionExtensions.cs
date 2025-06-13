using KeepItUp.MagJob.Identity.Core.Interfaces;

namespace KeepItUp.MagJob.Identity.Infrastructure.Keycloak;

/// <summary>
/// Extensions for IServiceCollection to register Keycloak services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Keycloak services to the DI container
    /// </summary>
    /// <param name="services">Collection of services</param>
    /// <param name="configuration">Configuration</param>
    /// <returns>Collection of services</returns>
    public static IServiceCollection AddKeycloakServices(this IServiceCollection services)
    {
        // Keycloak configuration options
        var serviceProvider = services.BuildServiceProvider();
        var keycloakAdminOptions = serviceProvider.GetRequiredService<IOptions<KeycloakAdminOptions>>().Value;

        // Register Keycloak client
        services.AddHttpClient<IKeycloakClient, KeycloakClient>((serviceProvider, client) =>
        {
            client.BaseAddress = new Uri(keycloakAdminOptions.ServerUrl);
            client.Timeout = TimeSpan.FromSeconds(keycloakAdminOptions.MaxTimeoutSeconds);
        });

        // Register HttpClient for Keycloak events
        services.AddHttpClient("KeycloakEvents", (serviceProvider, client) =>
        {
            client.BaseAddress = new Uri(keycloakAdminOptions.ServerUrl);
            client.Timeout = TimeSpan.FromSeconds(keycloakAdminOptions.MaxTimeoutSeconds);
        });

        // Register synchronization service
        services.AddScoped<IKeycloakSyncService, KeycloakSyncService>();

        // Register event listener
        services.AddHostedService<KeycloakEventListener>();

        return services;
    }
}
