using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using KeepItUp.MagJob.Identity.Infrastructure.Keycloak;
using Microsoft.Extensions.Options;

namespace KeepItUp.MagJob.Identity.Web.Services;

/// <summary>
/// Reprezentuje użytkownika w systemie Keycloak.
/// </summary>
public class KeycloakUser
{
    /// <summary>
    /// Identyfikator użytkownika.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Nazwa użytkownika.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Adres email użytkownika.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Imię użytkownika.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Nazwisko użytkownika.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Określa, czy konto użytkownika jest aktywne.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Określa, czy adres email użytkownika został zweryfikowany.
    /// </summary>
    public bool EmailVerified { get; set; }
}

/// <summary>
/// Reprezentuje rolę w systemie Keycloak.
/// </summary>
public class KeycloakRole
{
    /// <summary>
    /// Identyfikator roli.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Nazwa roli.
    /// </summary>
    public string? Name { get; set; }
}

/// <summary>
/// Reprezentuje odpowiedź z tokenem od serwera Keycloak.
/// </summary>
public class KeycloakTokenResponse
{
    /// <summary>
    /// Token dostępu.
    /// </summary>
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    /// <summary>
    /// Czas wygaśnięcia tokenu w sekundach.
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    /// <summary>
    /// Token odświeżania.
    /// </summary>
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Czas wygaśnięcia tokenu odświeżania w sekundach.
    /// </summary>
    [JsonPropertyName("refresh_expires_in")]
    public int RefreshExpiresIn { get; set; }

    /// <summary>
    /// Typ tokenu.
    /// </summary>
    [JsonPropertyName("token_type")]
    public string? TokenType { get; set; }

    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    [JsonPropertyName("session_state")]
    public string? SessionState { get; set; }

    [JsonPropertyName("not-before-policy")]
    public int NotBeforePolicy { get; set; }

    [JsonPropertyName("error")]
    public string? Error { get; set; }

    [JsonPropertyName("error_description")]
    public string? ErrorDescription { get; set; }
}

/// <summary>
/// Interfejs serwisu administracyjnego Keycloak.
/// </summary>
public interface IKeycloakAdminService
{
    /// <summary>
    /// Pobiera listę wszystkich użytkowników z Keycloak.
    /// </summary>
    /// <returns>Lista użytkowników Keycloak.</returns>
    Task<IEnumerable<KeycloakUser>> GetUsersAsync();

    /// <summary>
    /// Tworzy nowego użytkownika w Keycloak.
    /// </summary>
    /// <param name="username">Nazwa użytkownika.</param>
    /// <param name="email">Adres email użytkownika.</param>
    /// <param name="firstName">Imię użytkownika.</param>
    /// <param name="lastName">Nazwisko użytkownika.</param>
    /// <param name="password">Hasło użytkownika.</param>
    /// <param name="isEnabled">Określa, czy konto użytkownika ma być aktywne.</param>
    /// <returns>True, jeśli operacja zakończyła się sukcesem; w przeciwnym razie false.</returns>
    Task<bool> CreateUserAsync(string username, string email, string firstName, string lastName, string password, bool isEnabled = true);

    /// <summary>
    /// Aktualizuje dane użytkownika w Keycloak.
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika.</param>
    /// <param name="email">Nowy adres email użytkownika.</param>
    /// <param name="firstName">Nowe imię użytkownika.</param>
    /// <param name="lastName">Nowe nazwisko użytkownika.</param>
    /// <returns>True, jeśli operacja zakończyła się sukcesem; w przeciwnym razie false.</returns>
    Task<bool> UpdateUserAsync(string userId, string email, string firstName, string lastName);

    /// <summary>
    /// Usuwa użytkownika z Keycloak.
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika do usunięcia.</param>
    /// <returns>True, jeśli operacja zakończyła się sukcesem; w przeciwnym razie false.</returns>
    Task<bool> DeleteUserAsync(string userId);

    /// <summary>
    /// Przypisuje role do użytkownika w Keycloak.
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika.</param>
    /// <param name="roleNames">Lista nazw ról do przypisania.</param>
    /// <returns>True, jeśli operacja zakończyła się sukcesem; w przeciwnym razie false.</returns>
    Task<bool> AssignRolesToUserAsync(string userId, IEnumerable<string> roleNames);
}

/// <summary>
/// Implementacja serwisu administracyjnego Keycloak.
/// </summary>
public class KeycloakAdminService : IKeycloakAdminService
{
    /// <summary>
    /// Ustawienia administratora Keycloak.
    /// </summary>
    private readonly KeycloakAdminOptions _settings;

    /// <summary>
    /// Klient HTTP do komunikacji z API Keycloak.
    /// </summary>
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Logger dla klasy KeycloakAdminService.
    /// </summary>
    private readonly ILogger<KeycloakAdminService> _logger;

    /// <summary>
    /// Bieżący token dostępu do API Keycloak.
    /// </summary>
    private string? _accessToken;

    /// <summary>
    /// Data i czas wygaśnięcia tokenu dostępu.
    /// </summary>
    private DateTime _tokenExpiration = DateTime.MinValue;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="KeycloakAdminService"/>.
    /// </summary>
    /// <param name="settings">Opcje konfiguracyjne Keycloak.</param>
    /// <param name="httpClient">Klient HTTP do komunikacji z API Keycloak.</param>
    /// <param name="logger">Logger.</param>
    public KeycloakAdminService(IOptions<KeycloakAdminOptions> settings, HttpClient httpClient, ILogger<KeycloakAdminService> logger)
    {
        _settings = settings.Value;
        _httpClient = httpClient;
        _logger = logger;
        _httpClient.BaseAddress = new Uri(_settings.ServerUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds);
    }

    private async Task EnsureAuthenticatedAsync()
    {
        if (_accessToken != null && DateTime.UtcNow < _tokenExpiration)
        {
            return;
        }

        try
        {
            _logger.LogInformation("Attempting to obtain access token from Keycloak at {ServerUrl}", _settings.ServerUrl);

            // Używamy tokenu dla realm master, który jest używany do administracji
            var tokenEndpoint = $"/realms/master/protocol/openid-connect/token";

            _logger.LogDebug("Token endpoint: {TokenEndpoint}", tokenEndpoint);
            _logger.LogDebug("Admin username: {AdminUsername}, ClientId: {AdminClientId}", _settings.AdminUsername, _settings.AdminClientId);

            // Używamy FormUrlEncodedContent, który jest wymagany przez Keycloak
            var formData = new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "client_id", _settings.AdminClientId },
                { "username", _settings.AdminUsername },
                { "password", _settings.AdminPassword }
            };

            var content = new FormUrlEncodedContent(formData);

            // Dodajemy odpowiedni Content-Type header
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            _logger.LogDebug("Sending token request with data: {FormData}", JsonSerializer.Serialize(formData));

            var response = await _httpClient.PostAsync(tokenEndpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to obtain access token. Status code: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorContent);
                throw new InvalidOperationException($"Failed to obtain access token. Status code: {response.StatusCode}, Error: {errorContent}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogDebug("Token response: {Response}", responseContent);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = false // Wyłączamy case insensitive, ponieważ używamy JsonPropertyName
            };

            var tokenResponse = JsonSerializer.Deserialize<KeycloakTokenResponse>(responseContent, options);

            if (tokenResponse != null && !string.IsNullOrEmpty(tokenResponse.AccessToken))
            {
                _accessToken = tokenResponse.AccessToken;
                _tokenExpiration = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn - 30); // 30 seconds buffer
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
                _logger.LogInformation("Successfully obtained access token from Keycloak. Token expires at {ExpirationTime}", _tokenExpiration);
            }
            else
            {
                _logger.LogError("Token response was null or access token was empty");
                throw new InvalidOperationException("Failed to obtain access token from Keycloak: Token response was null or access token was empty");
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request error while obtaining access token: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to connect to Keycloak: {ex.Message}", ex);
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            _logger.LogError(ex, "Unexpected error while obtaining access token: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to obtain access token from Keycloak: {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<KeycloakUser>> GetUsersAsync()
    {
        try
        {
            await EnsureAuthenticatedAsync();

            var endpoint = $"/admin/realms/{_settings.Realm}/users";
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var users = await response.Content.ReadFromJsonAsync<List<KeycloakUser>>() ?? new List<KeycloakUser>();
            return users;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting users: {ex.Message}");
            return new List<KeycloakUser>();
        }
    }

    public async Task<bool> CreateUserAsync(string username, string email, string firstName, string lastName, string password, bool isEnabled = true)
    {
        try
        {
            await EnsureAuthenticatedAsync();

            var endpoint = $"/admin/realms/{_settings.Realm}/users";

            var user = new KeycloakUser
            {
                Username = username,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Enabled = isEnabled,
                EmailVerified = true
            };

            var response = await _httpClient.PostAsJsonAsync(endpoint, user);

            if (!response.IsSuccessStatusCode)
                return false;

            // Pobierz ID utworzonego użytkownika
            var locationHeader = response.Headers.Location;
            if (locationHeader == null)
                return false;

            var userId = locationHeader.Segments[^1];

            // Ustaw hasło
            return await SetPasswordAsync(userId, password);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating user: {ex.Message}");
            return false;
        }
    }

    private async Task<bool> SetPasswordAsync(string userId, string password)
    {
        try
        {
            var endpoint = $"/admin/realms/{_settings.Realm}/users/{userId}/reset-password";

            var passwordReset = new
            {
                type = "password",
                value = password,
                temporary = false
            };

            var response = await _httpClient.PutAsJsonAsync(endpoint, passwordReset);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting password: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UpdateUserAsync(string userId, string email, string firstName, string lastName)
    {
        try
        {
            await EnsureAuthenticatedAsync();

            var endpoint = $"/admin/realms/{_settings.Realm}/users/{userId}";

            // Pobierz aktualnego użytkownika
            var response = await _httpClient.GetAsync(endpoint);
            if (!response.IsSuccessStatusCode)
                return false;

            var user = await response.Content.ReadFromJsonAsync<KeycloakUser>();
            if (user == null)
                return false;

            // Aktualizuj dane
            user.Email = email;
            user.FirstName = firstName;
            user.LastName = lastName;

            // Wyślij aktualizację
            var updateResponse = await _httpClient.PutAsJsonAsync(endpoint, user);
            return updateResponse.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating user: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        try
        {
            await EnsureAuthenticatedAsync();

            var endpoint = $"/admin/realms/{_settings.Realm}/users/{userId}";
            var response = await _httpClient.DeleteAsync(endpoint);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting user: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> AssignRolesToUserAsync(string userId, IEnumerable<string> roleNames)
    {
        try
        {
            await EnsureAuthenticatedAsync();

            // Pobierz wszystkie role realm
            var rolesEndpoint = $"/admin/realms/{_settings.Realm}/roles";
            var rolesResponse = await _httpClient.GetAsync(rolesEndpoint);

            if (!rolesResponse.IsSuccessStatusCode)
                return false;

            var allRoles = await rolesResponse.Content.ReadFromJsonAsync<List<KeycloakRole>>() ?? new List<KeycloakRole>();

            // Filtruj role, które chcemy przypisać
            var rolesToAssign = allRoles.Where(r => roleNames.Contains(r.Name ?? "")).ToList();

            if (!rolesToAssign.Any())
                return false;

            // Przypisz role do użytkownika
            var assignEndpoint = $"/admin/realms/{_settings.Realm}/users/{userId}/role-mappings/realm";
            var assignResponse = await _httpClient.PostAsJsonAsync(assignEndpoint, rolesToAssign);

            return assignResponse.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error assigning roles: {ex.Message}");
            return false;
        }
    }
}
