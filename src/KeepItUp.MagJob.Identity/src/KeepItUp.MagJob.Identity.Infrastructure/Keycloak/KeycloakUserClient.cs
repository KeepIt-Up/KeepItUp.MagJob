using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using KeepItUp.MagJob.Identity.Core.Keycloak;

namespace KeepItUp.MagJob.Identity.Infrastructure.Keycloak;

/// <summary>
/// Client for managing users in Keycloak.
/// </summary>
public class KeycloakUserClient : BaseKeycloakClient
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KeycloakUserClient"/> class.
    /// </summary>
    /// <param name="httpClient">HTTP client.</param>
    /// <param name="options">Keycloak configuration options.</param>
    /// <param name="logger">Logger.</param>
    public KeycloakUserClient(
        HttpClient httpClient,
        IOptions<KeycloakAdminOptions> options,
        ILogger logger)
        : base(httpClient, options, logger)
    {
    }

    /// <summary>
    /// Gets a user from Keycloak based on the identifier.
    /// </summary>
    /// <param name="userId">User identifier in Keycloak.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>User data or null if the user does not exist.</returns>
    public async Task<KeycloakUser?> GetByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            await SetAuthorizationHeaderAsync(cancellationToken);

            var response = await HttpClient.GetAsync($"/admin/realms/{Options.Realm}/users/{userId}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<KeycloakUser>(cancellationToken: cancellationToken);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            Logger.LogError("Błąd podczas pobierania użytkownika z Keycloak. Status: {StatusCode}, Treść: {Content}",
                response.StatusCode, await response.Content.ReadAsStringAsync(cancellationToken));

            response.EnsureSuccessStatusCode();
            return null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Wystąpił błąd podczas pobierania użytkownika z Keycloak");
            throw;
        }
    }

    /// <summary>
    /// Gets a user from Keycloak based on the email address.
    /// </summary>
    /// <param name="email">User email address.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>User data or null if the user does not exist.</returns>
    public async Task<KeycloakUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            await SetAuthorizationHeaderAsync(cancellationToken);

            var response = await HttpClient.GetAsync($"/admin/realms/{Options.Realm}/users?email={Uri.EscapeDataString(email)}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var users = await response.Content.ReadFromJsonAsync<List<KeycloakUser>>(cancellationToken: cancellationToken);
                return users?.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            }

            Logger.LogError("Błąd podczas pobierania użytkownika z Keycloak. Status: {StatusCode}, Treść: {Content}",
                response.StatusCode, await response.Content.ReadAsStringAsync(cancellationToken));

            response.EnsureSuccessStatusCode();
            return null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Wystąpił błąd podczas pobierania użytkownika z Keycloak");
            throw;
        }
    }

    /// <summary>
    /// Gets a list of users from Keycloak.
    /// </summary>
    /// <param name="search">Optional search parameter.</param>
    /// <param name="first">Index of the first element to retrieve.</param>
    /// <param name="max">Maximum number of elements to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of users.</returns>
    public async Task<List<KeycloakUser>> GetUsersAsync(string? search = null, int first = 0, int max = 100, CancellationToken cancellationToken = default)
    {
        try
        {
            await SetAuthorizationHeaderAsync(cancellationToken);

            var url = $"/admin/realms/{Options.Realm}/users?first={first}&max={max}";
            if (!string.IsNullOrEmpty(search))
            {
                url += $"&search={Uri.EscapeDataString(search)}";
            }

            var response = await HttpClient.GetAsync(url, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<KeycloakUser>>(cancellationToken: cancellationToken) ?? new List<KeycloakUser>();
            }

            Logger.LogError("Błąd podczas pobierania użytkowników z Keycloak. Status: {StatusCode}, Treść: {Content}",
                response.StatusCode, await response.Content.ReadAsStringAsync(cancellationToken));

            response.EnsureSuccessStatusCode();
            return new List<KeycloakUser>();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Wystąpił błąd podczas pobierania użytkowników z Keycloak");
            throw;
        }
    }

    /// <summary>
    /// Creates a new user in Keycloak.
    /// </summary>
    /// <param name="user">User data to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>User identifier or null in case of an error.</returns>
    public async Task<string?> CreateUserAsync(KeycloakUser user, CancellationToken cancellationToken = default)
    {
        try
        {
            await SetAuthorizationHeaderAsync(cancellationToken);

            var response = await HttpClient.PostAsJsonAsync($"/admin/realms/{Options.Realm}/users", user, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                // Keycloak returns the location of the created user in the Location header
                var locationHeader = response.Headers.Location;
                if (locationHeader != null)
                {
                    var segments = locationHeader.Segments;
                    return segments[segments.Length - 1];
                }

                // If there is no Location header, get the user based on the email address
                var createdUser = await GetByEmailAsync(user.Email, cancellationToken);
                if (createdUser != null)
                {
                    return createdUser.Id;
                }

                Logger.LogWarning("Nie można pobrać identyfikatora utworzonego użytkownika");
                return null;
            }

            Logger.LogError("Błąd podczas tworzenia użytkownika w Keycloak. Status: {StatusCode}, Treść: {Content}",
                response.StatusCode, await response.Content.ReadAsStringAsync(cancellationToken));

            return null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Wystąpił błąd podczas tworzenia użytkownika w Keycloak");
            return null;
        }
    }

    /// <summary>
    /// Updates the user's data in Keycloak.
    /// </summary>
    /// <param name="user">User data to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the operation was successful; otherwise false.</returns>
    public async Task<bool> UpdateUserAsync(KeycloakUser user, CancellationToken cancellationToken = default)
    {
        try
        {
            await SetAuthorizationHeaderAsync(cancellationToken);

            var content = new StringContent(
                JsonSerializer.Serialize(user),
                Encoding.UTF8,
                "application/json");

            var response = await HttpClient.PutAsync(
                $"/admin/realms/{Options.Realm}/users/{user.Id}",
                content,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                Logger.LogError("Błąd podczas aktualizacji użytkownika w Keycloak. Status: {StatusCode}, Treść: {Content}",
                    response.StatusCode, await response.Content.ReadAsStringAsync(cancellationToken));
                return false;
            }

            Logger.LogInformation("Zaktualizowano użytkownika {UserId} w Keycloak", user.Id);
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Wystąpił błąd podczas aktualizacji użytkownika {UserId} w Keycloak", user.Id);
            return false;
        }
    }

    /// <summary>
    /// Updates the user's enabled status in Keycloak.
    /// </summary>
    /// <param name="userId">User identifier in Keycloak.</param>
    /// <param name="enabled">Whether the user should be enabled.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the operation was successful; otherwise false.</returns>
    public async Task<bool> UpdateUserEnabledStatusAsync(string userId, bool enabled, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                Logger.LogWarning("Użytkownik o ID {UserId} nie istnieje", userId);
                return false;
            }

            await SetAuthorizationHeaderAsync(cancellationToken);

            var updateUserRequest = new
            {
                enabled = enabled
            };

            var response = await HttpClient.PutAsJsonAsync($"/admin/realms/{Options.Realm}/users/{userId}", updateUserRequest, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                Logger.LogError("Błąd podczas aktualizacji statusu użytkownika w Keycloak. Status: {StatusCode}, Treść: {Content}",
                    response.StatusCode, await response.Content.ReadAsStringAsync(cancellationToken));
                return false;
            }

            Logger.LogInformation("Zaktualizowano status użytkownika {UserId} w Keycloak na {Status}", userId, enabled ? "aktywny" : "nieaktywny");
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Wystąpił błąd podczas aktualizacji statusu użytkownika {UserId} w Keycloak", userId);
            return false;
        }
    }

    /// <summary>
    /// Updates the user's attributes in Keycloak.
    /// </summary>
    /// <param name="userId">User identifier in Keycloak.</param>
    /// <param name="attributes">Attributes to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the operation was successful; otherwise false.</returns>
    public async Task<bool> UpdateUserAttributesAsync(string userId, Dictionary<string, List<string>> attributes, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                Logger.LogWarning("Użytkownik o ID {UserId} nie istnieje", userId);
                return false;
            }

            user.Attributes ??= new Dictionary<string, List<string>>();

            // Update or add new attributes
            foreach (var attribute in attributes)
            {
                user.Attributes[attribute.Key] = attribute.Value;
            }

            return await UpdateUserAsync(user, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Wystąpił błąd podczas aktualizacji atrybutów użytkownika {UserId} w Keycloak", userId);
            return false;
        }
    }

    /// <summary>
    /// Deactivates the user in Keycloak.
    /// </summary>
    /// <param name="userId">User identifier in Keycloak.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the operation was successful; otherwise false.</returns>
    public async Task<bool> DeactivateUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await UpdateUserEnabledStatusAsync(userId, false, cancellationToken);
    }

    /// <summary>
    /// Activates the user in Keycloak.
    /// </summary>
    /// <param name="userId">User identifier in Keycloak.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the operation was successful; otherwise false.</returns>
    public async Task<bool> ActivateUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await UpdateUserEnabledStatusAsync(userId, true, cancellationToken);
    }

    /// <summary>
    /// Gets all users from Keycloak.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of users.</returns>
    public async Task<List<KeycloakUser>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SetAuthorizationHeaderAsync(cancellationToken);

            var response = await HttpClient.GetAsync($"/admin/realms/{Options.Realm}/users?max=1000", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<KeycloakUser>>(cancellationToken: cancellationToken) ?? new List<KeycloakUser>();
            }

            Logger.LogError("Błąd podczas pobierania wszystkich użytkowników z Keycloak. Status: {StatusCode}, Treść: {Content}",
                response.StatusCode, await response.Content.ReadAsStringAsync(cancellationToken));

            response.EnsureSuccessStatusCode();
            return new List<KeycloakUser>();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Wystąpił błąd podczas pobierania wszystkich użytkowników z Keycloak");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<string?> GetUserProfilePictureUrlAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            await SetAuthorizationHeaderAsync(cancellationToken);

            var user = await GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                return null;
            }

            // Check if the user has the picture attribute
            if (user.Attributes != null && user.Attributes.TryGetValue("picture", out var pictures) && pictures.Count > 0)
            {
                return pictures[0];
            }

            // Check if the user has the avatar_url attribute (often used by GitHub)
            if (user.Attributes != null && user.Attributes.TryGetValue("avatar_url", out var avatarUrls) && avatarUrls.Count > 0)
            {
                return avatarUrls[0];
            }

            // Check if the user has the profile_picture attribute (used by some IDPs)
            if (user.Attributes != null && user.Attributes.TryGetValue("profile_picture", out var profilePictures) && profilePictures.Count > 0)
            {
                return profilePictures[0];
            }

            // Check if the user has the photo_url attribute (used by some IDPs)
            if (user.Attributes != null && user.Attributes.TryGetValue("photo_url", out var photoUrls) && photoUrls.Count > 0)
            {
                return photoUrls[0];
            }

            return null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Wystąpił błąd podczas pobierania zdjęcia profilowego użytkownika z Keycloak");
            return null;
        }
    }
}
