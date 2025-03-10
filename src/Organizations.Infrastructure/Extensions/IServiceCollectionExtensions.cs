using Microsoft.Extensions.DependencyInjection;
using Organizations.Infrastructure.Persistence;
using Organizations.Infrastructure.Persistence.Repository;
using Organizations.Persistence.Repository;

namespace Organizations.Application;

public static class ServiceExtensions
{
    /// <summary>
    /// Add infrastructure layer to the services collection.
    /// </summary>
    /// Add all the services that are needed for the infrastructure layer.
    /// <param name="services">The services collection.</param>
    public static void AddInfrastructureLayer(this IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IMemberRepository, MemberRepository>();
        services.AddScoped<IInvitationRepository, InvitationRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}
