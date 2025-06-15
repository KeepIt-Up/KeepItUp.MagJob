using KeepItUp.MagJob.Identity.Core.Interfaces;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;
using KeepItUp.MagJob.Identity.Infrastructure.Data;
using KeepItUp.MagJob.Identity.Infrastructure.Data.Config;
using KeepItUp.MagJob.Identity.Infrastructure.Data.Repositories;
using KeepItUp.MagJob.Identity.Infrastructure.FileStorage;
using KeepItUp.MagJob.Identity.Infrastructure.Keycloak;
using KeepItUp.MagJob.Identity.Infrastructure.Services;

namespace KeepItUp.MagJob.Identity.Infrastructure;
public static class InfrastructureServiceExtensions
{
  public static IServiceCollection AddInfrastructureServices(
    this IServiceCollection services,
    ConfigurationManager config,
    ILogger logger)
  {
    string? connectionString = config.GetConnectionString("DefaultConnection");
    Guard.Against.Null(connectionString);

    // Configuration of DbContext for PostgreSQL
    services.AddDbContext<AppDbContext>(options =>
      options.UseNpgsql(connectionString, npgsqlOptions =>
      {
        npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", DataSchemaConstants.IDENTITY_SCHEMA);
      }));

    services
        .AddSingleton<IFileStorageService, LocalFileStorageService>()
        .AddSingleton<IUserProfilePictureService, UserProfilePictureService>()
        .AddScoped<IFileValidationService, FileValidationService>()
        .AddScoped<IOrganizationRepository, OrganizationRepository>()
        .AddScoped<IUserRepository, UserRepository>();

    // Add Keycloak services
    services.AddKeycloakServices();

    // Add Mapster configuration
    services.AddMapsterConfiguration();

    logger.LogInformation("{Project} services registered", "Infrastructure");

    return services;
  }
}
