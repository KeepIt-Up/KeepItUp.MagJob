using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using KeepItUp.MagJob.Identity.Core.Keycloak;

namespace KeepItUp.MagJob.Identity.Infrastructure.Keycloak;

/// <summary>
/// Klient do zarządzania użytkownikami w Keycloak.
/// </summary>
public class KeycloakUserClient : BaseKeycloakClient
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="KeycloakUserClient"/>.
    /// </summary>
    /// <param name="httpClient">Klient HTTP.</param>
    /// <param name="options">Opcje konfiguracji Keycloak.</param>
    /// <param name="logger">Logger.</param>
    public KeycloakUserClient(
        HttpClient httpClient,
        IOptions<KeycloakAdminOptions> options,
        ILogger logger)
        : base(httpClient, options, logger)
    {
    }

    /// <summary>
    /// Pobiera użytkownika z Keycloak na podstawie identyfikatora.
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika w Keycloak.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Dane użytkownika lub null, jeśli użytkownik nie istnieje.</returns>
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
    /// Pobiera użytkownika z Keycloak na podstawie adresu email.
    /// </summary>
    /// <param name="email">Adres email użytkownika.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Dane użytkownika lub null, jeśli użytkownik nie istnieje.</returns>
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
    /// Pobiera listę użytkowników z Keycloak.
    /// </summary>
    /// <param name="search">Opcjonalny parametr wyszukiwania.</param>
    /// <param name="first">Indeks pierwszego elementu do pobrania.</param>
    /// <param name="max">Maksymalna liczba elementów do pobrania.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Lista użytkowników.</returns>
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
    /// Tworzy nowego użytkownika w Keycloak.
    /// </summary>
    /// <param name="user">Dane użytkownika do utworzenia.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Identyfikator utworzonego użytkownika lub null w przypadku błędu.</returns>
    public async Task<string?> CreateUserAsync(KeycloakUser user, CancellationToken cancellationToken = default)
    {
        try
        {
            await SetAuthorizationHeaderAsync(cancellationToken);

            var response = await HttpClient.PostAsJsonAsync($"/admin/realms/{Options.Realm}/users", user, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                // Keycloak zwraca lokalizację utworzonego użytkownika w nagłówku Location
                var locationHeader = response.Headers.Location;
                if (locationHeader != null)
                {
                    var segments = locationHeader.Segments;
                    return segments[segments.Length - 1];
                }

                // Jeśli nie ma nagłówka Location, pobierz użytkownika na podstawie adresu email
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
    /// Aktualizuje dane użytkownika w Keycloak.
    /// </summary>
    /// <param name="user">Dane użytkownika do aktualizacji.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>True, jeśli operacja zakończyła się sukcesem; w przeciwnym razie false.</returns>
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
    /// Aktualizuje status aktywności użytkownika w Keycloak.
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika w Keycloak.</param>
    /// <param name="enabled">Czy użytkownik ma być aktywny.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>True, jeśli operacja zakończyła się sukcesem; w przeciwnym razie false.</returns>
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
    /// Aktualizuje atrybuty użytkownika w Keycloak.
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika w Keycloak.</param>
    /// <param name="attributes">Atrybuty do zaktualizowania.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>True, jeśli operacja zakończyła się sukcesem; w przeciwnym razie false.</returns>
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

            // Aktualizuj lub dodaj nowe atrybuty
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
    /// Dezaktywuje użytkownika w Keycloak.
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika w Keycloak.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>True, jeśli operacja zakończyła się sukcesem; w przeciwnym razie false.</returns>
    public async Task<bool> DeactivateUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await UpdateUserEnabledStatusAsync(userId, false, cancellationToken);
    }

    /// <summary>
    /// Aktywuje użytkownika w Keycloak.
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika w Keycloak.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>True, jeśli operacja zakończyła się sukcesem; w przeciwnym razie false.</returns>
    public async Task<bool> ActivateUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await UpdateUserEnabledStatusAsync(userId, true, cancellationToken);
    }

    /// <summary>
    /// Pobiera wszystkich użytkowników z Keycloak.
    /// </summary>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Lista użytkowników.</returns>
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

            // Sprawdź, czy użytkownik ma atrybut picture
            if (user.Attributes != null && user.Attributes.TryGetValue("picture", out var pictures) && pictures.Count > 0)
            {
                return pictures[0];
            }

            // Sprawdź, czy użytkownik ma atrybut avatar_url (często używany przez GitHub)
            if (user.Attributes != null && user.Attributes.TryGetValue("avatar_url", out var avatarUrls) && avatarUrls.Count > 0)
            {
                return avatarUrls[0];
            }

            // Sprawdź, czy użytkownik ma atrybut profile_picture (używany przez niektóre IDP)
            if (user.Attributes != null && user.Attributes.TryGetValue("profile_picture", out var profilePictures) && profilePictures.Count > 0)
            {
                return profilePictures[0];
            }

            // Sprawdź, czy użytkownik ma atrybut photo_url (używany przez niektóre IDP)
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
