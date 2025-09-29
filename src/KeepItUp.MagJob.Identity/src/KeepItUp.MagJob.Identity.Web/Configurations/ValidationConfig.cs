using KeepItUp.MagJob.Identity.UseCases.Users.Commands.UpdateUser;

namespace KeepItUp.MagJob.Identity.Web.Configurations;

/// <summary>
/// Validation configuration for the application
/// </summary>
public static class ValidationConfig
{
    /// <summary>
    /// Adds validation configuration to the service collection
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="logger">Logger</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddValidationConfig(this IServiceCollection services, Microsoft.Extensions.Logging.ILogger logger)
    {
        services.AddValidatorsFromAssemblyContaining<UpdateUserCommandValidator>();

        logger.LogInformation("{Project} validation registered", "Web and UseCases validators");

        return services;
    }
}
