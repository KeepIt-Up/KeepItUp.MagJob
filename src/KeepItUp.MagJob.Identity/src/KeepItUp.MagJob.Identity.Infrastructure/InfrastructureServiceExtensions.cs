using KeepItUp.MagJob.Identity.Core.Interfaces;
using KeepItUp.MagJob.Identity.Core.Services;
using KeepItUp.MagJob.Identity.Infrastructure.Data;
using KeepItUp.MagJob.Identity.Infrastructure.Data.Config;
using KeepItUp.MagJob.Identity.Infrastructure.Keycloak;
using KeepItUp.MagJob.Identity.UseCases.Common;

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

    // Konfiguracja DbContext dla PostgreSQL
    services.AddDbContext<AppDbContext>(options =>
      options.UseNpgsql(connectionString, npgsqlOptions =>
      {
        npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", DataSchemaConstants.IDENTITY_SCHEMA);
      }));

    services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>))
           .AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>))
           .AddScoped(typeof(IEfRepository<>), typeof(EfRepository<>))
           .AddScoped<IDeleteContributorService, DeleteContributorService>();

    // Dodanie usług związanych z Keycloak
    services.AddKeycloakServices();

    // Dodanie konfiguracji Mapster
    services.AddMapsterConfiguration();

    logger.LogInformation("{Project} services registered", "Infrastructure");

    return services;
  }
}
