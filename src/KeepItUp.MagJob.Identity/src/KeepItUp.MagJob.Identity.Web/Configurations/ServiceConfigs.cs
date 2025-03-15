using KeepItUp.MagJob.Identity.Core.Interfaces;
using KeepItUp.MagJob.Identity.Infrastructure;
using KeepItUp.MagJob.Identity.Infrastructure.Email;
using KeepItUp.MagJob.Identity.Web.Services;

namespace KeepItUp.MagJob.Identity.Web.Configurations;

/// <summary>
/// Konfiguracja usług dla aplikacji
/// </summary>
public static class ServiceConfigs
{
  /// <summary>
  /// Dodaje konfigurację usług do kolekcji usług
  /// </summary>
  /// <param name="services">Kolekcja usług</param>
  /// <param name="logger">Logger</param>
  /// <param name="builder">Builder aplikacji</param>
  /// <returns>Kolekcja usług</returns>
  public static IServiceCollection AddServiceConfigs(this IServiceCollection services, Microsoft.Extensions.Logging.ILogger logger, WebApplicationBuilder builder)
  {
    // Dodaj FastEndpoints
    services.AddFastEndpoints();

    services.AddInfrastructureServices(builder.Configuration, logger)
            .AddMediatrConfigs();

    // Dodanie CORS
    services.AddCorsConfig(builder.Configuration);

    // Dodanie autoryzacji
    services.AddAuthorization();

    // Dodanie HttpContextAccessor i CurrentUserAccessor
    services.AddHttpContextAccessor();
    services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();

    // Dodanie HttpClient dla KeycloakAdminService
    services.AddHttpClient();
    services.AddScoped<IKeycloakAdminService, KeycloakAdminService>();

    if (builder.Environment.IsDevelopment())
    {
      // Use a local test email server
      // See: https://ardalis.com/configuring-a-local-test-email-server/
      services.AddScoped<IEmailSender, MimeKitEmailSender>();

      // Otherwise use this:
      //builder.Services.AddScoped<IEmailSender, FakeEmailSender>();

    }
    else
    {
      services.AddScoped<IEmailSender, MimeKitEmailSender>();
    }

    // Dodaj health checks
    builder.Services.AddHealthChecks();

    logger.LogInformation("{Project} services registered", "FastEndpoints, Mediatr, CORS, Authorization, CurrentUserAccessor, KeycloakAdmin and Email Sender");

    return services;
  }
}
