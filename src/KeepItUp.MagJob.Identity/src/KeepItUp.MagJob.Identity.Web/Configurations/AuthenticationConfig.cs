using KeepItUp.MagJob.Identity.Infrastructure.Keycloak;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace KeepItUp.MagJob.Identity.Web.Configurations;

public static class AuthenticationConfig
{
  /// <summary>
  /// Dodaje uwierzytelnianie JWT z Keycloak
  /// </summary>
  /// <param name="services">Kolekcja usług</param>
  /// <param name="configuration">Konfiguracja</param>
  /// <returns>Kolekcja usług</returns>
  public static IServiceCollection AddKeycloakAuthentication(this IServiceCollection services, IConfiguration configuration)
  {
    var keycloakOptions = configuration.GetSection("Keycloak").Get<KeycloakOptions>();
    if (keycloakOptions == null)
    {
      throw new InvalidOperationException("Brak konfiguracji Keycloak");
    }

    services.AddAuthentication(options =>
    {
      options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
      options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
      options.Authority = keycloakOptions.AuthorityUrl;
      options.Audience = keycloakOptions.ClientId;
      options.RequireHttpsMetadata = keycloakOptions.RequireHttps;
      options.SaveToken = true;
      options.TokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
      };
    });

    return services;
  }
}
