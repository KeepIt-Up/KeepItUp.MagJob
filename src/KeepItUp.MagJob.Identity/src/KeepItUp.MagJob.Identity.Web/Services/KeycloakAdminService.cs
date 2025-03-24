using KeepItUp.MagJob.Identity.Infrastructure.Keycloak;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KeepItUp.MagJob.Identity.Web.Services;

public class KeycloakUser
{
  public string? Id { get; set; }
  public string? Username { get; set; }
  public string? Email { get; set; }
  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public bool Enabled { get; set; }
  public bool EmailVerified { get; set; }
}

public class KeycloakRole
{
  public string? Id { get; set; }
  public string? Name { get; set; }
}

public class KeycloakTokenResponse
{
  [JsonPropertyName("access_token")]
  public string? AccessToken { get; set; }

  [JsonPropertyName("expires_in")]
  public int ExpiresIn { get; set; }

  [JsonPropertyName("refresh_token")]
  public string? RefreshToken { get; set; }

  [JsonPropertyName("refresh_expires_in")]
  public int RefreshExpiresIn { get; set; }

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

public interface IKeycloakAdminService
{
  Task<IEnumerable<KeycloakUser>> GetUsersAsync();
  Task<bool> CreateUserAsync(string username, string email, string firstName, string lastName, string password, bool isEnabled = true);
  Task<bool> UpdateUserAsync(string userId, string email, string firstName, string lastName);
  Task<bool> DeleteUserAsync(string userId);
  Task<bool> AssignRolesToUserAsync(string userId, IEnumerable<string> roleNames);
}

public class KeycloakAdminService : IKeycloakAdminService
{
  private readonly KeycloakAdminOptions _settings;
  private readonly HttpClient _httpClient;
  private readonly ILogger<KeycloakAdminService> _logger;
  private string? _accessToken;
  private DateTime _tokenExpiration = DateTime.MinValue;

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
