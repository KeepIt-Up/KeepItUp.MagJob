using Mapster;

namespace KeepItUp.MagJob.Identity.Infrastructure;

/// <summary>
/// Konfiguracja Mapster dla mapowania obiektów.
/// </summary>
public static class MapsterConfig
{
    /// <summary>
    /// Rejestruje konfigurację Mapster w kontenerze DI.
    /// </summary>
    /// <param name="services">Kolekcja usług.</param>
    /// <returns>Kolekcja usług z zarejestrowanym Mapster.</returns>
    public static IServiceCollection AddMapsterConfiguration(this IServiceCollection services)
    {
        // Konfiguracja globalnych ustawień Mapster
        var config = TypeAdapterConfig.GlobalSettings;
        config.Default.PreserveReference(true);

        return services;
    }
}
