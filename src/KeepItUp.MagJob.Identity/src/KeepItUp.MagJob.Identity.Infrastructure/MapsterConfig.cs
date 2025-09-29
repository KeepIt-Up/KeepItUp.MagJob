using Mapster;

namespace KeepItUp.MagJob.Identity.Infrastructure;

/// <summary>
/// Mapster configuration for mapping objects.
/// </summary>
public static class MapsterConfig
{
    /// <summary>
    /// Registers the Mapster configuration in the DI container.
    /// </summary>
    /// <param name="services">Collection of services.</param>
    /// <returns>Collection of services with Mapster registered.</returns>
    public static IServiceCollection AddMapsterConfiguration(this IServiceCollection services)
    {
        // Konfiguracja globalnych ustawień Mapster
        var config = TypeAdapterConfig.GlobalSettings;
        config.Default.PreserveReference(true);

        return services;
    }
}
