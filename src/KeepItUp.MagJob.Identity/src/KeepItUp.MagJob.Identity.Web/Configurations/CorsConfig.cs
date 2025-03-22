namespace KeepItUp.MagJob.Identity.Web.Configurations;

/// <summary>
/// Konfiguracja CORS dla aplikacji
/// </summary>
public static class CorsConfig
{
  /// <summary>
  /// Nazwa polityki CORS
  /// </summary>
  public const string CorsPolicyName = "DefaultCorsPolicy";

  /// <summary>
  /// Dodaje konfigurację CORS do kolekcji usług
  /// </summary>
  /// <param name="services">Kolekcja usług</param>
  /// <param name="configuration">Konfiguracja</param>
  /// <returns>Kolekcja usług</returns>
  public static IServiceCollection AddCorsConfig(this IServiceCollection services, IConfiguration configuration)
  {
    var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                          ?? throw new InvalidOperationException("Cors:AllowedOrigins is not set");

    services.AddCors(options =>
    {
      options.AddPolicy(CorsPolicyName, builder =>
      {
        builder
          .WithOrigins(allowedOrigins)
          .AllowAnyMethod()
          .AllowAnyHeader()
          .AllowCredentials();
      });
    });

    return services;
  }
}
