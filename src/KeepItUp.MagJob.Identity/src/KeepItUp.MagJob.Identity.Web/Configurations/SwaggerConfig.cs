using KeepItUp.MagJob.Identity.Infrastructure.Keycloak;
using NSwag;
using NSwag.AspNetCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using Microsoft.Extensions.Configuration;

namespace KeepItUp.MagJob.Identity.Web.Configurations;

/// <summary>
/// Konfiguracja Swagger dla aplikacji
/// </summary>
public static class SwaggerConfig
{
  /// <summary>
  /// Dodaje konfigurację Swagger do kolekcji usług
  /// </summary>
  /// <param name="services">Kolekcja usług</param>
  /// <param name="logger">Logger</param>
  /// <returns>Kolekcja usług</returns>
  public static IServiceCollection AddSwaggerConfig(this IServiceCollection services, Microsoft.Extensions.Logging.ILogger logger)
  {
    // Configure Swagger with OAuth2 authentication
    services.SwaggerDocument(o =>
    {
      o.ShortSchemaNames = true;
      o.DocumentSettings = s =>
          {
            s.Title = "MagJob Identity API";
            s.Version = "v1";

            // Pobierz konfigurację Keycloak dla klienta web
            var serviceProvider = services.BuildServiceProvider();
            var keycloakClientWeb = serviceProvider.GetRequiredService<IOptions<KeycloakClientOptions>>().Value;

            // Log Keycloak configuration for debugging
            logger.LogInformation("Keycloak client web configuration: ServerUrl={ServerUrl}, Realm={Realm}, ClientId={ClientId}",
                    keycloakClientWeb.ServerUrl, keycloakClientWeb.Realm, keycloakClientWeb.ClientId);

            // Configure OAuth2 security scheme
            s.AddAuth("Keycloak", new OpenApiSecurityScheme
            {
              Type = OpenApiSecuritySchemeType.OAuth2,
              Flows = new OpenApiOAuthFlows
              {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                  AuthorizationUrl = $"{keycloakClientWeb.ServerUrl}/realms/{keycloakClientWeb.Realm}/protocol/openid-connect/auth",
                  TokenUrl = $"{keycloakClientWeb.ServerUrl}/realms/{keycloakClientWeb.Realm}/protocol/openid-connect/token",
                  Scopes = new Dictionary<string, string>
                        {
                                { "openid", "OpenID Connect" },
                                { "profile", "User profile" },
                                { "email", "User email" }
                        }
                }
              },
              Description = "Keycloak Authentication"
            });
          };
    });

    return services;
  }

  /// <summary>
  /// Konfiguruje middleware Swagger
  /// </summary>
  /// <param name="app">Aplikacja</param>
  /// <param name="logger">Logger</param>
  /// <returns>Aplikacja</returns>
  public static IApplicationBuilder UseSwaggerConfig(this IApplicationBuilder app, Microsoft.Extensions.Logging.ILogger logger)
  {
    try
    {
      // Pobierz konfigurację Keycloak dla klienta web
      var keycloakClientWeb = app.ApplicationServices.GetRequiredService<IOptions<KeycloakClientOptions>>().Value;

      // Configure Swagger UI with OAuth2 client settings
      app.UseSwaggerGen(uiConfig: c =>
      {
        c.OAuth2Client = new OAuth2ClientSettings
        {
          ClientId = keycloakClientWeb.ClientId,
          ClientSecret = keycloakClientWeb.ClientSecret,
          AppName = "MagJob Identity API",
          Realm = keycloakClientWeb.Realm,
          UsePkceWithAuthorizationCodeGrant = true
        };

        // Add the redirect URL for Keycloak
        var configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();
        var applicationUrl = configuration["ApplicationUrl"];

        var redirectUrl = $"{applicationUrl}/swagger/oauth2-redirect.html";
        logger.LogInformation("OAuth2 redirect URL: {RedirectUrl}", redirectUrl);
        c.AdditionalSettings["oauth2RedirectUrl"] = redirectUrl;
      });

      return app;
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Błąd podczas konfiguracji Swagger UI");

      // Fallback configuration if there's an error
      app.UseSwaggerGen();
      return app;
    }
  }
}
