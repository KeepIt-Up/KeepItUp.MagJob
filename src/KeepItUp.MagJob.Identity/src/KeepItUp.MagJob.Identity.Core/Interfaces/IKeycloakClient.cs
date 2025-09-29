using KeepItUp.MagJob.Identity.Core.Keycloak;

namespace KeepItUp.MagJob.Identity.Core.Interfaces;

/// <summary>
/// Interface for communication with Keycloak API
/// </summary>
public interface IKeycloakClient
{
    /// <summary>
    /// Gets a user by their ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Keycloak user or null if not found</returns>
    Task<KeycloakUser?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by their email address
    /// </summary>
    /// <param name="email">User email address</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Keycloak user or null if not found</returns>
    Task<KeycloakUser?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets users based on search criteria
    /// </summary>
    /// <param name="search">Search phrase</param>
    /// <param name="first">Index of the first result</param>
    /// <param name="max">Maximum number of results</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of Keycloak users</returns>
    Task<List<KeycloakUser>> GetUsersAsync(string? search = null, int first = 0, int max = 100, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new user in Keycloak
    /// </summary>
    /// <param name="user">User data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ID of the created user</returns>
    Task<string> CreateUserAsync(KeycloakUser user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing user in Keycloak
    /// </summary>
    /// <param name="user">User data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UpdateUserAsync(KeycloakUser user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the enabled status of a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="enabled">New enabled status</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UpdateUserEnabledStatusAsync(string userId, bool enabled, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deactivates a user in Keycloak
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeactivateUserAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Activates a user in Keycloak
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ActivateUserAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the attributes of a user in Keycloak
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="attributes">Attributes to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UpdateUserAttributesAsync(string userId, Dictionary<string, List<string>> attributes, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an admin access token for Keycloak
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Admin access token</returns>
    Task<string> GetAdminAccessTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all users from Keycloak
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all users</returns>
    Task<List<KeycloakUser>> GetAllUsersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the roles of a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of user roles</returns>
    Task<List<string>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns a role to a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="roleName">Role name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task AssignRoleToUserAsync(string userId, string roleName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a role from a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="roleName">Role name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RemoveRoleFromUserAsync(string userId, string roleName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all available roles from Keycloak
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of roles</returns>
    Task<List<KeycloakRole>> GetRolesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new role in Keycloak
    /// </summary>
    /// <param name="role">Role data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ID of the created role</returns>
    Task<string> CreateRoleAsync(KeycloakRole role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing role in Keycloak
    /// </summary>
    /// <param name="roleName">Name of the role to update</param>
    /// <param name="role">New role data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UpdateRoleAsync(string roleName, KeycloakRole role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a role from Keycloak
    /// </summary>
    /// <param name="roleName">Name of the role to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteRoleAsync(string roleName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the URL of the user's profile picture from Keycloak/IDP
    /// </summary>
    /// <param name="userId">User ID in Keycloak</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>URL of the user's profile picture or null if the user has no picture</returns>
    Task<string?> GetUserProfilePictureUrlAsync(string userId, CancellationToken cancellationToken = default);
}
