using KeepItUp.MagJob.Identity.Core.Interfaces;

namespace KeepItUp.MagJob.Identity.IntegrationTests.Infrastructure;

/// <summary>
/// Mock implementation of IKeycloakSyncService for integration tests.
/// </summary>
public class MockKeycloakSyncService : IKeycloakSyncService
{
    public Task SyncUserRolesAsync(string userId, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task SyncUserDataAsync(string userId, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task SyncAllUsersAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task<Guid> ImportUserFromKeycloakAsync(string keycloakUserId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Guid.NewGuid());
    }
}