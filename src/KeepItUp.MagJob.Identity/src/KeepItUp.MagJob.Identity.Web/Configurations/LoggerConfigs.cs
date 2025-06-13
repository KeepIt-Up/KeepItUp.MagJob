namespace KeepItUp.MagJob.Identity.Web.Configurations;

/// <summary>
/// Logger configuration for the application
/// </summary>
public static class LoggerConfigs
{
    /// <summary>
    /// Adds logger configuration to the application
    /// </summary>
    /// <param name="builder">Web application builder</param>
    /// <returns>Web application builder</returns>
    public static WebApplicationBuilder AddLoggerConfigs(this WebApplicationBuilder builder)
    {

        builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration));

        return builder;
    }
}
