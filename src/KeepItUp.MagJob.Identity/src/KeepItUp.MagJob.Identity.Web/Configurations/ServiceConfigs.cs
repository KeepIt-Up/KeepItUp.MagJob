using KeepItUp.MagJob.Identity.Core.Interfaces;
using KeepItUp.MagJob.Identity.Infrastructure;
using KeepItUp.MagJob.Identity.Infrastructure.Email;
using KeepItUp.MagJob.Identity.Web.Services;

namespace KeepItUp.MagJob.Identity.Web.Configurations;

/// <summary>
/// Service configuration for the application
/// </summary>
public static class ServiceConfigs
{
    /// <summary>
    /// Adds service configuration to the service collection
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="logger">Logger</param>
    /// <param name="builder">Builder application</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddServiceConfigs(this IServiceCollection services, Microsoft.Extensions.Logging.ILogger logger, WebApplicationBuilder builder)
    {
        services.AddFastEndpoints();

        services.AddInfrastructureServices(builder.Configuration, logger)
                .AddMediatrConfigs()
                .AddValidationConfig(logger);

        services.AddCorsConfig(builder.Configuration);

        services.AddAuthorization();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();

        services.AddHttpClient();
        services.AddScoped<IKeycloakAdminService, KeycloakAdminService>();

        services.AddScoped<IEmailSender, MimeKitEmailSender>();

        services.AddHealthCheckConfigs(builder.Configuration, logger);

        logger.LogInformation("{Project} services registered", "FastEndpoints, Mediatr, Validation, CORS, Authorization, CurrentUserAccessor, KeycloakAdmin, Email Sender and Health Checks");

        return services;
    }
}
