using MediatR;
using Microsoft.Extensions.DependencyInjection;
using KeepItUp.MagJob.Identity.Infrastructure.Data;

namespace KeepItUp.MagJob.Identity.IntegrationTests.Infrastructure;

/// <summary>
/// Base class for integration tests with database and MediatR setup.
/// </summary>
public abstract class BaseIntegrationTest : IClassFixture<DatabaseFixture>, IAsyncLifetime
{
    protected readonly DatabaseFixture DatabaseFixture;
    protected readonly TestWebApplicationFactory Factory;
    protected readonly IServiceScope Scope;
    protected readonly AppDbContext DbContext;
    protected readonly IMediator Mediator;

    protected BaseIntegrationTest(DatabaseFixture databaseFixture)
    {
        DatabaseFixture = databaseFixture;
        Factory = new TestWebApplicationFactory(databaseFixture);
        Scope = Factory.Services.CreateScope();
        DbContext = Scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Mediator = Scope.ServiceProvider.GetRequiredService<IMediator>();
    }

    public virtual async Task InitializeAsync()
    {
        await Factory.ResetDatabaseAsync();
    }

    public virtual async Task DisposeAsync()
    {
        Scope?.Dispose();
        if (Factory != null)
            await Factory.DisposeAsync();
    }

    /// <summary>
    /// Helper method to save changes and clear change tracker.
    /// </summary>
    protected async Task SaveAndClearAsync()
    {
        await DbContext.SaveChangesAsync();
        DbContext.ChangeTracker.Clear();
    }
}