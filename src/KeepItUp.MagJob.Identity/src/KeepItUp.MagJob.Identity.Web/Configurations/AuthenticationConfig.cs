using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using KeepItUp.MagJob.Identity.Infrastructure.Keycloak;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace KeepItUp.MagJob.Identity.Web.Configurations;

public static class AuthenticationConfig
{
  /// <summary>
  /// Dodaje uwierzytelnianie JWT z Keycloak
  /// </summary>
  /// <param name="services">Kolekcja usług</param>
  /// <returns>Kolekcja usług</returns>
  public static IServiceCollection AddKeycloakAuthentication(this IServiceCollection services)
  {
    // Pobierz konfigurację Keycloak dla klienta web
    var serviceProvider = services.BuildServiceProvider();
    var keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakAdminOptions>>().Value;

    if (keycloakOptions == null)
    {
      throw new InvalidOperationException("Brak konfiguracji Keycloak");
    }

    // Wyłącz domyślne mapowanie claims, aby uniknąć duplikatów
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

      // Wyłącz automatyczne mapowanie claims, aby mieć pełną kontrolę nad procesem
      options.MapInboundClaims = false;

      // Konfiguracja obsługi zdarzeń JWT Bearer
      options.Events = new JwtBearerEvents
      {
        OnTokenValidated = context =>
        {
          // Obsługa duplikatów claims - ten krok jest kluczowy dla poprawnego działania autentykacji
          // Keycloak może zwracać duplikaty niektórych typów claims, co powoduje błędy podczas walidacji
          var claimsIdentity = context.Principal?.Identity as ClaimsIdentity;
          if (claimsIdentity != null)
          {
            // Utwórz słownik do śledzenia już przetworzonych typów claims
            var processedClaimTypes = new HashSet<string>();

            // Utwórz listę claims do usunięcia
            var claimsToRemove = new List<Claim>();

            // Dla każdego claim w tożsamości
            foreach (var claim in claimsIdentity.Claims.ToList())
            {
              // Jeśli ten typ claim już widzieliśmy, dodaj go do listy do usunięcia
              if (!processedClaimTypes.Add(claim.Type))
              {
                claimsToRemove.Add(claim);
              }
            }

            // Usuń duplikaty
            foreach (var claim in claimsToRemove)
            {
              claimsIdentity.RemoveClaim(claim);
            }
          }

          return Task.CompletedTask;
        }
      };

      // Konfiguracja parametrów walidacji tokenu
      options.TokenValidationParameters = new TokenValidationParameters
      {
        // Włącz wszystkie standardowe walidacje
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        // Obsługa różnych issuerów (localhost i keycloak)
        ValidIssuers = new[]
        {
          keycloakOptions.AuthorityUrl,
          $"http://localhost:18080/realms/{keycloakOptions.Realm}",
          $"http://keycloak:8080/realms/{keycloakOptions.Realm}"
        },

        // Ustaw dozwolone audience - ważne dla poprawnej walidacji tokenu
        ValidAudiences = new[]
        {
          keycloakOptions.ClientId,
          "account",
          "client.web"
        },

        // Ustaw typy claims dla ról i nazwy użytkownika
        RoleClaimType = "roles",
        NameClaimType = "preferred_username"
      };
    });

    return services;
  }
}
