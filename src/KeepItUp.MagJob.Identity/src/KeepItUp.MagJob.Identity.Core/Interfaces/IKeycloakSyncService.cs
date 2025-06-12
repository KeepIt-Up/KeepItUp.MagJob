

namespace KeepItUp.MagJob.Identity.Core.Interfaces;

/// <summary>
/// Interface for synchronization with Keycloak
/// </summary>
public interface IKeycloakSyncService
{
    /// <summary>
    /// Synchronizes user roles with Keycloak
    /// </summary>
    /// <param name="userId">User ID in Keycloak</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing an asynchronous operation</returns>
    Task SyncUserRolesAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Synchronizes user data with Keycloak
    /// </summary>
    /// <param name="userId">User ID in Keycloak</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing an asynchronous operation</returns>
    Task SyncUserDataAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Synchronizes all users from Keycloak
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing an asynchronous operation</returns>
    Task SyncAllUsersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Imports a new user from Keycloak to the Identity module
    /// </summary>
    /// <param name="keycloakUserId">User ID in Keycloak</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ID of the created user in the Identity module</returns>
    Task<Guid> ImportUserFromKeycloakAsync(string keycloakUserId, CancellationToken cancellationToken = default);
}
