using Microsoft.Extensions.DependencyInjection;
using Organizations.Application.Common.Behaviors;
using Organizations.Application.Common.Interfaces;
using System.Reflection;

namespace Organizations.Application;

public static class ServiceExtensions
{
    /// <summary>
    /// Add application layer to the services collection.
    /// </summary>
    /// Add all the services that are needed for the application layer.
    /// <param name="services">The services collection.</param>
    public static void AddApplicationLayer(this IServiceCollection services)
    {
        services.AddMapster();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddValidatorsFromAssemblies(new[] { Assembly.GetExecutingAssembly() });
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    }
}
