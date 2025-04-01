using KeepItUp.MagJob.Identity.Core.Interfaces;

namespace KeepItUp.MagJob.Identity.Infrastructure.Keycloak;

/// <summary>
/// Rozszerzenia dla IServiceCollection do rejestracji usług związanych z Keycloak
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Dodaje usługi związane z Keycloak do kontenera DI
    /// </summary>
    /// <param name="services">Kolekcja usług</param>
    /// <param name="configuration">Konfiguracja</param>
    /// <returns>Kolekcja usług</returns>
    public static IServiceCollection AddKeycloakServices(this IServiceCollection services)
    {
        // Konfiguracja opcji Keycloak
        var serviceProvider = services.BuildServiceProvider();
        var keycloakAdminOptions = serviceProvider.GetRequiredService<IOptions<KeycloakAdminOptions>>().Value;

        // Rejestracja klienta Keycloak
        services.AddHttpClient<IKeycloakClient, KeycloakClient>((serviceProvider, client) =>
        {
            client.BaseAddress = new Uri(keycloakAdminOptions.ServerUrl);
            client.Timeout = TimeSpan.FromSeconds(keycloakAdminOptions.MaxTimeoutSeconds);
        });

        // Rejestracja HttpClient dla zdarzeń Keycloak
        services.AddHttpClient("KeycloakEvents", (serviceProvider, client) =>
        {
            client.BaseAddress = new Uri(keycloakAdminOptions.ServerUrl);
            client.Timeout = TimeSpan.FromSeconds(keycloakAdminOptions.MaxTimeoutSeconds);
        });

        // Rejestracja serwisu synchronizacji
        services.AddScoped<IKeycloakSyncService, KeycloakSyncService>();

        // Rejestracja nasłuchiwacza zdarzeń
        services.AddHostedService<KeycloakEventListener>();

        return services;
    }
}
