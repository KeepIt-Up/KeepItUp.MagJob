using KeepItUp.MagJob.Identity.Infrastructure.Data.Config;
using Microsoft.EntityFrameworkCore.Design;

namespace KeepItUp.MagJob.Identity.Infrastructure.Data;

/// <summary>
/// Factory class for creating instances of AppDbContext during design-time operations
/// like migrations, scaffolding, etc.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // Get the current directory and try to find the Web project directory
        var baseDirectory = Directory.GetCurrentDirectory();
        var webProjectPath = Path.Combine(baseDirectory, "..", "KeepItUp.MagJob.Identity.Web");

        if (!Directory.Exists(webProjectPath))
        {
            // If we're in the Web project already, use that
            webProjectPath = baseDirectory;
        }

        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(webProjectPath)
            .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        var configuration = configurationBuilder.Build();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            // Fallback connection string for design-time operations
            connectionString = "Host=localhost;Database=magjob;Username=postgres;Password=postgres";
        }

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connectionString, npgsqlOptions =>
        {
            npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", DataSchemaConstants.IDENTITY_SCHEMA);
        });

        // Create AppDbContext without domain event dispatcher for design-time operations
        return new AppDbContext(optionsBuilder.Options, null);
    }
}
