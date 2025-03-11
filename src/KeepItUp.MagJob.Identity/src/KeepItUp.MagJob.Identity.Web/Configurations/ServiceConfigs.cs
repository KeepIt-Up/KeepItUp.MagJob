using KeepItUp.MagJob.Identity.Core.Interfaces;
using KeepItUp.MagJob.Identity.Infrastructure;
using KeepItUp.MagJob.Identity.Infrastructure.Email;
using KeepItUp.MagJob.Identity.Web.Services;
using Microsoft.AspNetCore.Http;

namespace KeepItUp.MagJob.Identity.Web.Configurations;

public static class ServiceConfigs
{
  public static IServiceCollection AddServiceConfigs(this IServiceCollection services, Microsoft.Extensions.Logging.ILogger logger, WebApplicationBuilder builder)
  {
    services.AddInfrastructureServices(builder.Configuration, logger)
            .AddMediatrConfigs();

    // Dodanie HttpContextAccessor i CurrentUserAccessor
    services.AddHttpContextAccessor();
    services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();

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

    logger.LogInformation("{Project} services registered", "Mediatr, CurrentUserAccessor and Email Sender");

    return services;
  }
}
