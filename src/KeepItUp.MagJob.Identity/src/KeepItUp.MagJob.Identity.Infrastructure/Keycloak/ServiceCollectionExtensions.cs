using Microsoft.AspNetCore.Authentication;

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
  public static IServiceCollection AddKeycloakServices(this IServiceCollection services, IConfiguration configuration)
  {
    // Konfiguracja opcji Keycloak
    services.Configure<KeycloakOptions>(configuration.GetSection("Keycloak"));

    // Rejestracja klienta Keycloak
    services.AddHttpClient<IKeycloakClient, KeycloakClient>();
    services.AddScoped<IKeycloakClient, KeycloakClient>();

    // Rejestracja serwisu synchronizacji
    services.AddScoped<IKeycloakSyncService, KeycloakSyncService>();

    // Rejestracja nasłuchiwacza zdarzeń
    //services.AddHostedService<KeycloakEventListener>();

    // Rejestracja transformacji claimów
    services.AddTransient<IClaimsTransformation, KeycloakAttributeMapper.KeycloakClaimsTransformation>();

    return services;
  }
}
