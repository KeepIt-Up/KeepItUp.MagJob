namespace KeepItUp.MagJob.Identity.Web.Configurations;

public static class CorsConfig
{
  public const string CorsPolicyName = "MagJobCorsPolicy";

  /// <summary>
  /// Dodaje konfigurację CORS do usług
  /// </summary>
  /// <param name="services">Kolekcja usług</param>
  /// <param name="configuration">Konfiguracja</param>
  /// <returns>Kolekcja usług</returns>
  public static IServiceCollection AddCorsConfiguration(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddCors(options =>
    {
      options.AddPolicy(name: CorsPolicyName,
              policy =>
              {

                var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

                if (allowedOrigins == null)
                {
                  throw new InvalidOperationException("Brak konfiguracji dozwolonych źródeł");
                }

                policy.WithOrigins(allowedOrigins)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
              });
    });

    return services;
  }
}
