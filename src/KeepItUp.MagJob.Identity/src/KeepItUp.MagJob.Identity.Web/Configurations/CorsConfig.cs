namespace KeepItUp.MagJob.Identity.Web.Configurations;

/// <summary>
/// CORS configuration for the application
/// </summary>
public static class CorsConfig
{
    /// <summary>
    /// CORS policy name
    /// </summary>
    public const string CorsPolicyName = "DefaultCorsPolicy";

    /// <summary>
    /// Adds CORS configuration to the service collection
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration</param>
    /// <returns>Service collection</returns>
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
