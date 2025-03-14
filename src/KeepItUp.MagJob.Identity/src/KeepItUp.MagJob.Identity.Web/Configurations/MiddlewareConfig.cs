using Ardalis.ListStartupServices;
using KeepItUp.MagJob.Identity.Infrastructure.Data;

namespace KeepItUp.MagJob.Identity.Web.Configurations;

/// <summary>
/// Konfiguracja middleware dla aplikacji
/// </summary>
public static class MiddlewareConfig
{
  /// <summary>
  /// Konfiguruje middleware aplikacji i inicjalizuje bazę danych
  /// </summary>
  /// <param name="app">Aplikacja webowa</param>
  /// <returns>Skonfigurowana aplikacja</returns>
  public static async Task<IApplicationBuilder> UseAppMiddlewareAndSeedDatabase(this WebApplication app)
  {
    if (app.Environment.IsDevelopment())
    {
      app.UseDeveloperExceptionPage();
      app.UseShowAllServicesMiddleware(); // see https://github.com/ardalis/AspNetCoreStartupServices
    }
    else
    {
      app.UseDefaultExceptionHandler(); // from FastEndpoints
      app.UseHsts();
    }

    app.UseCors(CorsConfig.CorsPolicyName);

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseFastEndpoints(c =>
    {
      // Ustawienie PropertyNamingPolicy na null powoduje, że nazwy właściwości w JSON
      // są zachowywane dokładnie tak, jak w klasach C# (PascalCase).
      // Jest to zgodne z konwencją .NET, ale różni się od standardu JSON (camelCase).
      // Uwaga: Jeśli klienci oczekują camelCase, należy zmienić to ustawienie na:
      // c.Serializer.Options.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
      c.Serializer.Options.PropertyNamingPolicy = null;
    });

    app.UseHttpsRedirection();

    await SeedDatabase(app);

    return app;
  }

  /// <summary>
  /// Inicjalizuje bazę danych i wypełnia ją danymi początkowymi
  /// </summary>
  /// <param name="app">Aplikacja webowa</param>
  static async Task SeedDatabase(WebApplication app)
  {
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    try
    {
      var context = services.GetRequiredService<AppDbContext>();

      context.Database.EnsureCreated();

      await SeedData.InitializeAsync(context);
    }
    catch (Exception ex)
    {
      var logger = services.GetRequiredService<ILogger<Program>>();
      logger.LogError(ex, "An error occurred seeding the DB. {exceptionMessage}", ex.Message);
    }
  }
}
