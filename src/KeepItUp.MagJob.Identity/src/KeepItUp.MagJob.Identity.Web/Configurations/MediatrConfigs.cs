using System.Reflection;
using Ardalis.SharedKernel;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.CreateOrganization;

namespace KeepItUp.MagJob.Identity.Web.Configurations;

/// <summary>
/// MediatR configuration for the application
/// </summary>
public static class MediatrConfigs
{
  /// <summary>
  /// Adds MediatR configuration to the service collection
  /// </summary>
  /// <param name="services">Service collection</param>
  /// <returns>Service collection</returns>
  public static IServiceCollection AddMediatrConfigs(this IServiceCollection services)
  {
    var mediatRAssemblies = new[]
              {
        Assembly.GetAssembly(typeof(Organization)), // Core
        Assembly.GetAssembly(typeof(CreateOrganizationCommand)) // UseCases
      };

    services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(mediatRAssemblies!))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
            .AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();

    return services;
  }
}
