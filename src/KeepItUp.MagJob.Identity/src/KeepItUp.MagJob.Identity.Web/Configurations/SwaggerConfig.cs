using KeepItUp.MagJob.Identity.Infrastructure.Keycloak;
using Microsoft.Extensions.Options;
using NSwag;
using NSwag.AspNetCore;

namespace KeepItUp.MagJob.Identity.Web.Configurations;

/// <summary>
/// Swagger configuration for the application
/// </summary>
public static class SwaggerConfig
{
    /// <summary>
    /// Adds Swagger configuration to the service collection
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="logger">Logger</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddSwaggerConfig(this IServiceCollection services, Microsoft.Extensions.Logging.ILogger logger)
    {
        services.SwaggerDocument(o =>
        {
            o.ShortSchemaNames = true;
            o.DocumentSettings = s =>
            {
                s.Title = "MagJob Identity API";
                s.Version = "v1";

                var serviceProvider = services.BuildServiceProvider();
                var keycloakClientWeb = serviceProvider.GetRequiredService<IOptions<KeycloakClientOptions>>().Value;

                logger.LogInformation("Keycloak client web configuration: ServerUrl={ServerUrl}, Realm={Realm}, ClientId={ClientId}",
                    keycloakClientWeb.ServerUrl, keycloakClientWeb.Realm, keycloakClientWeb.ClientId);

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
    /// Configures Swagger middleware
    /// </summary>
    /// <param name="app">Application</param>
    /// <param name="logger">Logger</param>
    /// <returns>Application</returns>
    public static IApplicationBuilder UseSwaggerConfig(this IApplicationBuilder app, Microsoft.Extensions.Logging.ILogger logger)
    {
        try
        {
            var keycloakClientWeb = app.ApplicationServices.GetRequiredService<IOptions<KeycloakClientOptions>>().Value;

            app.UseSwaggerGen(uiConfig: c =>
            {
                c.OAuth2Client = new OAuth2ClientSettings
                {
                    ClientId = keycloakClientWeb.ClientId,
                    AppName = "MagJob Identity API",
                    Realm = keycloakClientWeb.Realm,
                    UsePkceWithAuthorizationCodeGrant = true
                };

                var configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();
                var httpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();

                var request = httpContextAccessor?.HttpContext?.Request;
                if (request == null)
                {
                    throw new InvalidOperationException("HttpContextAccessor nie zwrócił prawidłowego żądania");
                }

                var applicationUrl = $"{request.Scheme}://{request.Host}";

                var redirectUrl = $"{applicationUrl}/swagger/oauth2-redirect.html";
                logger.LogInformation("OAuth2 redirect URL: {RedirectUrl}", redirectUrl);
                c.AdditionalSettings["oauth2RedirectUrl"] = redirectUrl;
            });

            return app;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Błąd podczas konfiguracji Swagger UI");

            app.UseSwaggerGen();
            return app;
        }
    }
}
