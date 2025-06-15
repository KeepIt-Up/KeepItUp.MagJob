using Ardalis.ListStartupServices;
using KeepItUp.MagJob.Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace KeepItUp.MagJob.Identity.Web.Configurations;

/// <summary>
/// Middleware configuration for the application
/// </summary>
public static class MiddlewareConfig
{
    /// <summary>
    /// Configures middleware for the application and initializes the database
    /// </summary>
    /// <param name="app">Web application</param>
    /// <returns>Configured application</returns>
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

        app.MapHealthCheckEndpoints();

        app.UseCors(CorsConfig.CorsPolicyName);

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseFastEndpoints(c =>
       {
           c.Serializer.Options.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
       });

        app.UseHttpsRedirection();

        await SeedDatabase(app);

        return app;
    }

    /// <summary>
    /// Initializes the database and seeds it with initial data
    /// </summary>
    /// <param name="app">Web application</param>
    static async Task SeedDatabase(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var context = services.GetRequiredService<AppDbContext>();
            var logger = services.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("Applying migrations...");
            await context.Database.MigrateAsync();
            logger.LogInformation("Migrations applied successfully");

            await SeedData.InitializeAsync(context);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred seeding the DB. {exceptionMessage}", ex.Message);
        }
    }
}
