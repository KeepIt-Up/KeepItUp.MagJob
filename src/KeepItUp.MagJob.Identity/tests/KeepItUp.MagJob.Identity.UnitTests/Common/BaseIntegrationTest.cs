using Microsoft.Extensions.DependencyInjection;

namespace KeepItUp.MagJob.Identity.UnitTests.Common;

/// <summary>
/// Base class for integration tests that require dependency injection.
/// For tests requiring database access, use the BaseIntegrationTest in IntegrationTests project.
/// </summary>
public abstract class BaseIntegrationTest : BaseUnitTest, IAsyncLifetime
{
    /// <summary>
    /// Service collection for dependency injection in tests.
    /// </summary>
    protected IServiceCollection Services { get; }

    /// <summary>
    /// Service provider built from Services collection.
    /// </summary>
    protected IServiceProvider ServiceProvider { get; private set; } = null!;

    /// <summary>
    /// Initializes the integration test.
    /// </summary>
    protected BaseIntegrationTest()
    {
        Services = new ServiceCollection();
    }

    /// <summary>
    /// Initialize the test environment.
    /// Configures services and builds service provider.
    /// </summary>
    public virtual async Task InitializeAsync()
    {
        ConfigureServices(Services);
        ServiceProvider = Services.BuildServiceProvider();

        await SetupAsync();
    }

    /// <summary>
    /// Override this method to configure services for the test.
    /// </summary>
    /// <param name="services">Service collection to configure</param>
    protected virtual void ConfigureServices(IServiceCollection services)
    {
        // Override in derived classes to add specific services
    }

    /// <summary>
    /// Override this method to perform additional setup after service configuration.
    /// </summary>
    protected virtual async Task SetupAsync()
    {
        // Override in derived classes for additional setup
        await Task.CompletedTask;
    }

    /// <summary>
    /// Gets a service from the service provider.
    /// </summary>
    /// <typeparam name="T">Type of service to get</typeparam>
    /// <returns>Service instance</returns>
    protected T GetService<T>() where T : notnull
    {
        return ServiceProvider.GetRequiredService<T>();
    }

    /// <summary>
    /// Gets a service from the service provider or null if not registered.
    /// </summary>
    /// <typeparam name="T">Type of service to get</typeparam>
    /// <returns>Service instance or null</returns>
    protected T? GetOptionalService<T>() where T : class
    {
        return ServiceProvider.GetService<T>();
    }

    /// <summary>
    /// Creates a new scope for dependency injection.
    /// </summary>
    /// <returns>Service scope</returns>
    protected IServiceScope CreateScope()
    {
        return ServiceProvider.CreateScope();
    }

    /// <summary>
    /// Cleanup the test environment.
    /// </summary>
    public virtual async Task DisposeAsync()
    {
        if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
        await Task.CompletedTask;
    }

    /// <summary>
    /// Cleanup resources.
    /// </summary>
    public override void Dispose()
    {
        // Dispose is handled by DisposeAsync
        base.Dispose();
    }
}