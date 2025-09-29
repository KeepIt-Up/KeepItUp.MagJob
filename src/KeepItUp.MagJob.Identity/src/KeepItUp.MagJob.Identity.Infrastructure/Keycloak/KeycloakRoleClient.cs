using System.Net.Http.Json;
using KeepItUp.MagJob.Identity.Core.Keycloak;

namespace KeepItUp.MagJob.Identity.Infrastructure.Keycloak;

/// <summary>
/// Client for managing roles in Keycloak.
/// </summary>
public class KeycloakRoleClient : BaseKeycloakClient
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KeycloakRoleClient"/> class.
    /// </summary>
    /// <param name="httpClient">HTTP client.</param>
    /// <param name="options">Keycloak configuration options.</param>
    /// <param name="logger">Logger.</param>
    public KeycloakRoleClient(
        HttpClient httpClient,
        IOptions<KeycloakAdminOptions> options,
        ILogger logger)
        : base(httpClient, options, logger)
    {
    }

    /// <summary>
    /// Gets all available roles from Keycloak.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of available roles.</returns>
    public async Task<List<KeycloakRole>> GetRolesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SetAuthorizationHeaderAsync(cancellationToken);

            var response = await HttpClient.GetAsync($"/admin/realms/{Options.Realm}/roles", cancellationToken);
            var roles = await HandleResponseAsync<List<KeycloakRole>>(
                response,
                "Błąd podczas pobierania ról",
                cancellationToken);

            return roles ?? new List<KeycloakRole>();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Nieoczekiwany błąd podczas pobierania ról: {Message}", ex.Message);
            return new List<KeycloakRole>();
        }
    }

    /// <summary>
    /// Gets the user's roles from Keycloak.
    /// </summary>
    /// <param name="userId">User ID in Keycloak.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of user roles.</returns>
    public async Task<List<string>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            await SetAuthorizationHeaderAsync(cancellationToken);

            var response = await HttpClient.GetAsync($"/admin/realms/{Options.Realm}/users/{userId}/role-mappings/realm", cancellationToken);
            var roles = await HandleResponseAsync<List<KeycloakRole>>(
                response,
                $"Błąd podczas pobierania ról użytkownika {userId}",
                cancellationToken);

            return roles?.Select(r => r.Name).ToList() ?? new List<string>();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Nieoczekiwany błąd podczas pobierania ról użytkownika {UserId}: {Message}", userId, ex.Message);
            return new List<string>();
        }
    }

    /// <summary>
    /// Creates a new role in Keycloak.
    /// </summary>
    /// <param name="role">Role data to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>ID of the created role or null in case of an error.</returns>
    public async Task<string?> CreateRoleAsync(KeycloakRole role, CancellationToken cancellationToken = default)
    {
        try
        {
            await SetAuthorizationHeaderAsync(cancellationToken);

            var response = await HttpClient.PostAsJsonAsync($"/admin/realms/{Options.Realm}/roles", role, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                Logger.LogError("Błąd podczas tworzenia roli {RoleName}. Status: {StatusCode}. Treść: {ErrorContent}",
                    role.Name, response.StatusCode, errorContent);
                return null;
            }

            // Get the ID of the created role
            var createdRole = await GetRoleByNameAsync(role.Name, cancellationToken);
            return createdRole?.Id;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Nieoczekiwany błąd podczas tworzenia roli {RoleName}: {Message}", role.Name, ex.Message);
            return null;
        }
    }

    /// <summary>
    /// Gets a role by name.
    /// </summary>
    /// <param name="roleName">Role name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Role data or null if the role does not exist.</returns>
    public async Task<KeycloakRole?> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default)
    {
        try
        {
            await SetAuthorizationHeaderAsync(cancellationToken);

            var response = await HttpClient.GetAsync($"/admin/realms/{Options.Realm}/roles/{roleName}", cancellationToken);
            return await HandleResponseAsync<KeycloakRole>(
                response,
                $"Błąd podczas pobierania roli {roleName}",
                cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Nieoczekiwany błąd podczas pobierania roli {RoleName}: {Message}", roleName, ex.Message);
            return null;
        }
    }

    /// <summary>
    /// Updates a role in Keycloak.
    /// </summary>
    /// <param name="roleName">Role name.</param>
    /// <param name="role">Updated role data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the operation was successful; otherwise false.</returns>
    public async Task<bool> UpdateRoleAsync(string roleName, KeycloakRole role, CancellationToken cancellationToken = default)
    {
        try
        {
            await SetAuthorizationHeaderAsync(cancellationToken);

            var response = await HttpClient.PutAsJsonAsync($"/admin/realms/{Options.Realm}/roles/{roleName}", role, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                Logger.LogError("Błąd podczas aktualizacji roli {RoleName}. Status: {StatusCode}. Treść: {ErrorContent}",
                    roleName, response.StatusCode, errorContent);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Nieoczekiwany błąd podczas aktualizacji roli {RoleName}: {Message}", roleName, ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Deletes a role from Keycloak.
    /// </summary>
    /// <param name="roleName">Role name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the operation was successful; otherwise false.</returns>
    public async Task<bool> DeleteRoleAsync(string roleName, CancellationToken cancellationToken = default)
    {
        try
        {
            await SetAuthorizationHeaderAsync(cancellationToken);

            var response = await HttpClient.DeleteAsync($"/admin/realms/{Options.Realm}/roles/{roleName}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                Logger.LogError("Błąd podczas usuwania roli {RoleName}. Status: {StatusCode}. Treść: {ErrorContent}",
                    roleName, response.StatusCode, errorContent);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Nieoczekiwany błąd podczas usuwania roli {RoleName}: {Message}", roleName, ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Assigns a role to a user in Keycloak.
    /// </summary>
    /// <param name="userId">User ID in Keycloak.</param>
    /// <param name="roleName">Role name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the operation was successful; otherwise false.</returns>
    public async Task<bool> AssignRoleToUserAsync(string userId, string roleName, CancellationToken cancellationToken = default)
    {
        try
        {
            var role = await GetRoleByNameAsync(roleName, cancellationToken);
            if (role == null)
            {
                return false;
            }

            await SetAuthorizationHeaderAsync(cancellationToken);

            var roles = new List<KeycloakRole> { role };
            var response = await HttpClient.PostAsJsonAsync(
                $"/admin/realms/{Options.Realm}/users/{userId}/role-mappings/realm",
                roles,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                Logger.LogError("Błąd podczas przypisywania roli {RoleName} do użytkownika {UserId}. Status: {StatusCode}. Treść: {ErrorContent}",
                    roleName, userId, response.StatusCode, errorContent);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Nieoczekiwany błąd podczas przypisywania roli {RoleName} do użytkownika {UserId}: {Message}",
                roleName, userId, ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Removes a role from a user in Keycloak.
    /// </summary>
    /// <param name="userId">User ID in Keycloak.</param>
    /// <param name="roleName">Role name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the operation was successful; otherwise false.</returns>
    public async Task<bool> RemoveRoleFromUserAsync(string userId, string roleName, CancellationToken cancellationToken = default)
    {
        try
        {
            var role = await GetRoleByNameAsync(roleName, cancellationToken);
            if (role == null)
            {
                return false;
            }

            await SetAuthorizationHeaderAsync(cancellationToken);

            var roles = new List<KeycloakRole> { role };
            var request = new HttpRequestMessage(HttpMethod.Delete, $"/admin/realms/{Options.Realm}/users/{userId}/role-mappings/realm")
            {
                Content = JsonContent.Create(roles)
            };

            var response = await HttpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                Logger.LogError("Błąd podczas usuwania roli {RoleName} użytkownika {UserId}. Status: {StatusCode}. Treść: {ErrorContent}",
                    roleName, userId, response.StatusCode, errorContent);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Nieoczekiwany błąd podczas usuwania roli {RoleName} użytkownika {UserId}: {Message}",
                roleName, userId, ex.Message);
            return false;
        }
    }
}
