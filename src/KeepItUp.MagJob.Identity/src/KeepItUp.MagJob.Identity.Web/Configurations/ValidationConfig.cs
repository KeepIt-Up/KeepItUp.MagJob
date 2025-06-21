using KeepItUp.MagJob.Identity.UseCases.Users.Commands.UpdateUser;

namespace KeepItUp.MagJob.Identity.Web.Configurations;

/// <summary>
/// Konfiguracja walidacji dla aplikacji
/// </summary>
public static class ValidationConfig
{
    /// <summary>
    /// Dodaje konfigurację walidacji do kolekcji usług
    /// </summary>
    /// <param name="services">Kolekcja usług</param>
    /// <param name="logger">Logger</param>
    /// <returns>Kolekcja usług</returns>
    public static IServiceCollection AddValidationConfig(this IServiceCollection services, Microsoft.Extensions.Logging.ILogger logger)
    {
        // Rejestracja walidatorów FastEndpoints (warstwa Web)
        // Działa automatycznie - FastEndpoints odkrywa walidatory zgodnie z konwencją nazewnictwa

        // Rejestracja walidatorów FluentValidation (warstwa UseCases)
        services.AddValidatorsFromAssemblyContaining<UpdateUserCommandValidator>();

        logger.LogInformation("{Project} validation registered", "Web and UseCases validators");

        return services;
    }
}
