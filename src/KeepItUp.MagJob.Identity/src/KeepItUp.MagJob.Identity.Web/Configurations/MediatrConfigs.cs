using Ardalis.SharedKernel;
using KeepItUp.MagJob.Identity.Core.ContributorAggregate;
using KeepItUp.MagJob.Identity.UseCases.Contributors.Create;
using MediatR;
using System.Reflection;

namespace KeepItUp.MagJob.Identity.Web.Configurations;

public static class MediatrConfigs
{
  public static IServiceCollection AddMediatrConfigs(this IServiceCollection services)
  {
    var mediatRAssemblies = new[]
      {
        Assembly.GetAssembly(typeof(Contributor)), // Core
        Assembly.GetAssembly(typeof(CreateContributorCommand)) // UseCases
      };

    services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(mediatRAssemblies!))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
            .AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();

    return services;
  }
}
