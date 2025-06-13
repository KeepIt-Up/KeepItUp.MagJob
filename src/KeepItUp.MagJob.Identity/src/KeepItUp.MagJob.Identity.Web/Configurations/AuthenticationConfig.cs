using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using KeepItUp.MagJob.Identity.Infrastructure.Keycloak;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace KeepItUp.MagJob.Identity.Web.Configurations;

/// <summary>
/// Authentication configuration for the application
/// </summary>
public static class AuthenticationConfig
{
    /// <summary>
    /// Adds JWT authentication with Keycloak
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddKeycloakAuthentication(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakAdminOptions>>().Value;

        if (keycloakOptions == null)
        {
            throw new InvalidOperationException("Brak konfiguracji Keycloak");
        }

        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.Authority = keycloakOptions.AuthorityUrl;
            options.RequireHttpsMetadata = keycloakOptions.RequireHttps;
            options.SaveToken = true;

            options.MapInboundClaims = false;

            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = context =>
            {
                var claimsIdentity = context.Principal?.Identity as ClaimsIdentity;
                if (claimsIdentity != null)
                {
                    var processedClaimTypes = new HashSet<string>();

                    var claimsToRemove = new List<Claim>();

                    foreach (var claim in claimsIdentity.Claims.ToList())
                    {
                        if (!processedClaimTypes.Add(claim.Type))
                        {
                            claimsToRemove.Add(claim);
                        }
                    }

                    foreach (var claim in claimsToRemove)
                    {
                        claimsIdentity.RemoveClaim(claim);
                    }
                }

                return Task.CompletedTask;
            }
            };

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuers = new[]
          {
          keycloakOptions.AuthorityUrl,
          $"http://localhost:18080/realms/{keycloakOptions.Realm}",
          $"http://keycloak:8080/realms/{keycloakOptions.Realm}"
            },

                ValidAudiences = new[]
          {
          keycloakOptions.ClientId,
          "account",
          "keepitup-magjob-client"
            },

                RoleClaimType = "roles",
                NameClaimType = "preferred_username"
            };
        });

        return services;
    }
}
