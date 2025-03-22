using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using KeepItUp.MagJob.Identity.Core.Interfaces;
using KeepItUp.MagJob.Identity.Core.Keycloak;


namespace KeepItUp.MagJob.Identity.Infrastructure.Keycloak;

/// <summary>
/// Implementacja klienta do komunikacji z API Keycloak
/// </summary>
public class KeycloakClient : IKeycloakClient
{
  private readonly HttpClient _httpClient;
  private readonly KeycloakAdminOptions _options;
  private readonly ILogger<KeycloakClient> _logger;
  private string? _cachedToken;
  private DateTime _tokenExpiration = DateTime.MinValue;

  /// <summary>
  /// Inicjalizuje nową instancję klasy <see cref="KeycloakClient"/>
  /// </summary>
  /// <param name="httpClient">Klient HTTP</param>
  /// <param name="options">Opcje konfiguracji Keycloak</param>
  /// <param name="logger">Logger</param>
  public KeycloakClient(
      HttpClient httpClient,
      IOptions<KeycloakAdminOptions> options,
      ILogger<KeycloakClient> logger)
  {
    _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    _httpClient.BaseAddress = new Uri(_options.ServerUrl);
    _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
  }

  /// <inheritdoc />
  public async Task<KeycloakUser?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
  {
    try
    {
      var token = await GetAdminAccessTokenAsync(cancellationToken);
      _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

      var response = await _httpClient.GetAsync($"/admin/realms/{_options.Realm}/users/{userId}", cancellationToken);

      if (response.IsSuccessStatusCode)
      {
        return await response.Content.ReadFromJsonAsync<KeycloakUser>(cancellationToken: cancellationToken);
      }

      if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
      {
        return null;
      }

      _logger.LogError("Błąd podczas pobierania użytkownika z Keycloak. Status: {StatusCode}, Treść: {Content}",
          response.StatusCode, await response.Content.ReadAsStringAsync(cancellationToken));

      response.EnsureSuccessStatusCode();
      return null;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Wystąpił błąd podczas pobierania użytkownika z Keycloak");
      throw;
    }
  }

  /// <inheritdoc />
  public async Task<KeycloakUser?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
  {
    try
    {
      var token = await GetAdminAccessTokenAsync(cancellationToken);
      _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

      var response = await _httpClient.GetAsync($"/admin/realms/{_options.Realm}/users?email={Uri.EscapeDataString(email)}", cancellationToken);

      if (response.IsSuccessStatusCode)
      {
        var users = await response.Content.ReadFromJsonAsync<List<KeycloakUser>>(cancellationToken: cancellationToken);
        return users?.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
      }

      _logger.LogError("Błąd podczas pobierania użytkownika z Keycloak. Status: {StatusCode}, Treść: {Content}",
          response.StatusCode, await response.Content.ReadAsStringAsync(cancellationToken));

      response.EnsureSuccessStatusCode();
      return null;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Wystąpił błąd podczas pobierania użytkownika z Keycloak");
      throw;
    }
  }

  /// <inheritdoc />
  public async Task<List<KeycloakUser>> GetUsersAsync(string? search = null, int first = 0, int max = 100, CancellationToken cancellationToken = default)
  {
    try
    {
      var token = await GetAdminAccessTokenAsync(cancellationToken);
      _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

      var url = $"/admin/realms/{_options.Realm}/users?first={first}&max={max}";
      if (!string.IsNullOrEmpty(search))
      {
        url += $"&search={Uri.EscapeDataString(search)}";
      }

      var response = await _httpClient.GetAsync(url, cancellationToken);

      if (response.IsSuccessStatusCode)
      {
        return await response.Content.ReadFromJsonAsync<List<KeycloakUser>>(cancellationToken: cancellationToken) ?? new List<KeycloakUser>();
      }

      _logger.LogError("Błąd podczas pobierania użytkowników z Keycloak. Status: {StatusCode}, Treść: {Content}",
          response.StatusCode, await response.Content.ReadAsStringAsync(cancellationToken));

      response.EnsureSuccessStatusCode();
      return new List<KeycloakUser>();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Wystąpił błąd podczas pobierania użytkowników z Keycloak");
      throw;
    }
  }

  /// <inheritdoc />
  public async Task<string> CreateUserAsync(KeycloakUser user, CancellationToken cancellationToken = default)
  {
    try
    {
      var token = await GetAdminAccessTokenAsync(cancellationToken);
      _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

      var response = await _httpClient.PostAsJsonAsync($"/admin/realms/{_options.Realm}/users", user, cancellationToken);

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
        var createdUser = await GetUserByEmailAsync(user.Email, cancellationToken);
        if (createdUser != null)
        {
          return createdUser.Id;
        }

        throw new InvalidOperationException("Nie można pobrać identyfikatora utworzonego użytkownika");
      }

      _logger.LogError("Błąd podczas tworzenia użytkownika w Keycloak. Status: {StatusCode}, Treść: {Content}",
          response.StatusCode, await response.Content.ReadAsStringAsync(cancellationToken));

      response.EnsureSuccessStatusCode();
      return string.Empty;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Wystąpił błąd podczas tworzenia użytkownika w Keycloak");
      throw;
    }
  }

  /// <inheritdoc />
  public async Task UpdateUserAsync(KeycloakUser user, CancellationToken cancellationToken = default)
  {
    try
    {
      var token = await GetAdminAccessTokenAsync(cancellationToken);
      _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

      var content = new StringContent(
          JsonSerializer.Serialize(user),
          Encoding.UTF8,
          "application/json");

      var response = await _httpClient.PutAsync(
          $"/admin/realms/{_options.Realm}/users/{user.Id}",
          content,
          cancellationToken);

      if (!response.IsSuccessStatusCode)
      {
        _logger.LogError("Błąd podczas aktualizacji użytkownika w Keycloak. Status: {StatusCode}, Treść: {Content}",
            response.StatusCode, await response.Content.ReadAsStringAsync(cancellationToken));
      }

      response.EnsureSuccessStatusCode();

      _logger.LogInformation("Zaktualizowano użytkownika {UserId} w Keycloak", user.Id);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Wystąpił błąd podczas aktualizacji użytkownika {UserId} w Keycloak", user.Id);
      throw;
    }
  }

  /// <inheritdoc />
  public async Task DeactivateUserAsync(string userId, CancellationToken cancellationToken = default)
  {
    try
    {
      var user = await GetUserByIdAsync(userId, cancellationToken);
      if (user == null)
      {
        throw new InvalidOperationException($"Użytkownik o identyfikatorze {userId} nie istnieje w Keycloak");
      }

      user.Enabled = false;
      await UpdateUserAsync(user, cancellationToken);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Wystąpił błąd podczas dezaktywacji użytkownika w Keycloak");
      throw;
    }
  }

  /// <inheritdoc />
  public async Task ActivateUserAsync(string userId, CancellationToken cancellationToken = default)
  {
    try
    {
      var user = await GetUserByIdAsync(userId, cancellationToken);
      if (user == null)
      {
        throw new InvalidOperationException($"Użytkownik o identyfikatorze {userId} nie istnieje w Keycloak");
      }

      user.Enabled = true;
      await UpdateUserAsync(user, cancellationToken);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Wystąpił błąd podczas aktywacji użytkownika w Keycloak");
      throw;
    }
  }

  /// <inheritdoc />
  public async Task UpdateUserAttributesAsync(string userId, Dictionary<string, List<string>> attributes, CancellationToken cancellationToken = default)
  {
    try
    {
      var user = await GetUserByIdAsync(userId, cancellationToken);
      if (user == null)
      {
        throw new InvalidOperationException($"Użytkownik o identyfikatorze {userId} nie istnieje w Keycloak");
      }

      user.Attributes ??= new Dictionary<string, List<string>>();

      // Aktualizuj lub dodaj nowe atrybuty
      foreach (var attribute in attributes)
      {
        user.Attributes[attribute.Key] = attribute.Value;
      }

      await UpdateUserAsync(user, cancellationToken);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Wystąpił błąd podczas aktualizacji atrybutów użytkownika w Keycloak");
      throw;
    }
  }

  /// <inheritdoc />
  public async Task<string> GetAdminAccessTokenAsync(CancellationToken cancellationToken = default)
  {
    // Sprawdź, czy token jest ważny
    if (!string.IsNullOrEmpty(_cachedToken) && _tokenExpiration > DateTime.UtcNow)
    {
      return _cachedToken;
    }

    try
    {
      // Użyj danych uwierzytelniających klienta usługi
      _logger.LogDebug("Próba uwierzytelnienia jako klient usługi Keycloak: {ClientId}", _options.ClientId);

      var clientContent = new FormUrlEncodedContent(new[]
      {
                new KeyValuePair<string, string>("client_id", _options.ClientId),
                new KeyValuePair<string, string>("client_secret", _options.ClientSecret),
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            });

      var clientResponse = await _httpClient.PostAsync($"/realms/{_options.Realm}/protocol/openid-connect/token", clientContent, cancellationToken);

      if (clientResponse.IsSuccessStatusCode)
      {
        var tokenResponse = await clientResponse.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
        var accessToken = tokenResponse.GetProperty("access_token").GetString();
        var expiresIn = tokenResponse.GetProperty("expires_in").GetInt32();

        if (string.IsNullOrEmpty(accessToken))
        {
          throw new InvalidOperationException("Otrzymano pusty token dostępu z Keycloak");
        }

        _logger.LogDebug("Pomyślnie pobrano token klienta usługi Keycloak");

        // Zapisz token i czas jego wygaśnięcia
        _cachedToken = accessToken;
        _tokenExpiration = DateTime.UtcNow.AddSeconds(expiresIn - 30); // Odejmij 30 sekund dla bezpieczeństwa

        return accessToken;
      }

      _logger.LogError("Błąd podczas pobierania tokenu dostępu z Keycloak. Status: {StatusCode}, Treść: {Content}",
          clientResponse.StatusCode, await clientResponse.Content.ReadAsStringAsync(cancellationToken));

      clientResponse.EnsureSuccessStatusCode();
      return string.Empty;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Wystąpił błąd podczas pobierania tokenu dostępu z Keycloak");
      throw;
    }
  }

  /// <inheritdoc />
  public async Task<List<string>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default)
  {
    try
    {
      var token = await GetAdminAccessTokenAsync(cancellationToken);
      _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

      var response = await _httpClient.GetAsync($"/admin/realms/{_options.Realm}/users/{userId}/role-mappings/realm", cancellationToken);

      if (response.IsSuccessStatusCode)
      {
        var roles = await response.Content.ReadFromJsonAsync<List<KeycloakRole>>(cancellationToken: cancellationToken);
        return roles?.Select(r => r.Name).ToList() ?? new List<string>();
      }

      _logger.LogError("Błąd podczas pobierania ról użytkownika z Keycloak. Status: {StatusCode}, Treść: {Content}",
          response.StatusCode, await response.Content.ReadAsStringAsync(cancellationToken));

      response.EnsureSuccessStatusCode();
      return new List<string>();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Wystąpił błąd podczas pobierania ról użytkownika z Keycloak");
      throw;
    }
  }

  /// <inheritdoc />
  public async Task AssignRoleToUserAsync(string userId, string roleName, CancellationToken cancellationToken = default)
  {
    try
    {
      var token = await GetAdminAccessTokenAsync(cancellationToken);
      _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

      // Pobierz wszystkie role
      var allRoles = await GetRolesAsync(cancellationToken);
      var role = allRoles.FirstOrDefault(r => r.Name == roleName);

      if (role == null)
      {
        throw new InvalidOperationException($"Rola {roleName} nie istnieje w Keycloak");
      }

      var content = new StringContent(
          JsonSerializer.Serialize(new[] { role }),
          Encoding.UTF8,
          "application/json");

      var response = await _httpClient.PostAsync(
          $"/admin/realms/{_options.Realm}/users/{userId}/role-mappings/realm",
          content,
          cancellationToken);

      if (!response.IsSuccessStatusCode)
      {
        _logger.LogError("Błąd podczas przypisywania roli do użytkownika w Keycloak. Status: {StatusCode}, Treść: {Content}",
            response.StatusCode, await response.Content.ReadAsStringAsync(cancellationToken));
      }

      response.EnsureSuccessStatusCode();

      _logger.LogInformation("Przypisano rolę {RoleName} do użytkownika {UserId} w Keycloak", roleName, userId);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Wystąpił błąd podczas przypisywania roli {RoleName} do użytkownika {UserId} w Keycloak", roleName, userId);
      throw;
    }
  }

  /// <inheritdoc />
  public async Task RemoveRoleFromUserAsync(string userId, string roleName, CancellationToken cancellationToken = default)
  {
    try
    {
      var token = await GetAdminAccessTokenAsync(cancellationToken);
      _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

      // Pobierz wszystkie role
      var allRoles = await GetRolesAsync(cancellationToken);
      var role = allRoles.FirstOrDefault(r => r.Name == roleName);

      if (role == null)
      {
        throw new InvalidOperationException($"Rola {roleName} nie istnieje w Keycloak");
      }

      var content = new StringContent(
          JsonSerializer.Serialize(new[] { role }),
          Encoding.UTF8,
          "application/json");

      var request = new HttpRequestMessage
      {
        Method = HttpMethod.Delete,
        RequestUri = new Uri($"/admin/realms/{_options.Realm}/users/{userId}/role-mappings/realm", UriKind.Relative),
        Content = content
      };

      var response = await _httpClient.SendAsync(request, cancellationToken);

      if (!response.IsSuccessStatusCode)
      {
        _logger.LogError("Błąd podczas usuwania roli użytkownika w Keycloak. Status: {StatusCode}, Treść: {Content}",
            response.StatusCode, await response.Content.ReadAsStringAsync(cancellationToken));
      }

      response.EnsureSuccessStatusCode();

      _logger.LogInformation("Usunięto rolę {RoleName} użytkownika {UserId} w Keycloak", roleName, userId);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Wystąpił błąd podczas usuwania roli {RoleName} użytkownika {UserId} w Keycloak", roleName, userId);
      throw;
    }
  }

  /// <inheritdoc />
  public async Task<List<KeycloakRole>> GetRolesAsync(CancellationToken cancellationToken = default)
  {
    try
    {
      var token = await GetAdminAccessTokenAsync(cancellationToken);
      _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

      var response = await _httpClient.GetAsync($"/admin/realms/{_options.Realm}/roles", cancellationToken);

      if (response.IsSuccessStatusCode)
      {
        return await response.Content.ReadFromJsonAsync<List<KeycloakRole>>(cancellationToken: cancellationToken) ?? new List<KeycloakRole>();
      }

      _logger.LogError("Błąd podczas pobierania ról z Keycloak. Status: {StatusCode}, Treść: {Content}",
          response.StatusCode, await response.Content.ReadAsStringAsync(cancellationToken));

      response.EnsureSuccessStatusCode();
      return new List<KeycloakRole>();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Wystąpił błąd podczas pobierania ról z Keycloak");
      throw;
    }
  }

  /// <inheritdoc />
  public async Task<string> CreateRoleAsync(KeycloakRole role, CancellationToken cancellationToken = default)
  {
    try
    {
      var token = await GetAdminAccessTokenAsync(cancellationToken);
      _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

      var content = new StringContent(
          JsonSerializer.Serialize(role),
          Encoding.UTF8,
          "application/json");

      var response = await _httpClient.PostAsync(
          $"/admin/realms/{_options.Realm}/roles",
          content,
          cancellationToken);

      if (!response.IsSuccessStatusCode)
      {
        _logger.LogError("Błąd podczas tworzenia roli w Keycloak. Status: {StatusCode}, Treść: {Content}",
            response.StatusCode, await response.Content.ReadAsStringAsync(cancellationToken));

        response.EnsureSuccessStatusCode();
      }

      // Pobierz identyfikator utworzonej roli
      var locationHeader = response.Headers.Location;
      if (locationHeader != null)
      {
        var segments = locationHeader.Segments;
        var roleId = segments[segments.Length - 1];
        return roleId;
      }

      // Jeśli nie ma nagłówka Location, pobierz rolę na podstawie nazwy
      var roles = await GetRolesAsync(cancellationToken);
      var createdRole = roles.FirstOrDefault(r => r.Name == role.Name);

      if (createdRole != null && !string.IsNullOrEmpty(createdRole.Id))
      {
        return createdRole.Id;
      }

      throw new InvalidOperationException("Nie udało się pobrać identyfikatora utworzonej roli");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Wystąpił błąd podczas tworzenia roli w Keycloak");
      throw;
    }
  }

  /// <inheritdoc />
  public async Task UpdateRoleAsync(string roleName, KeycloakRole role, CancellationToken cancellationToken = default)
  {
    try
    {
      var token = await GetAdminAccessTokenAsync(cancellationToken);
      _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

      var content = new StringContent(
          JsonSerializer.Serialize(role),
          Encoding.UTF8,
          "application/json");

      var response = await _httpClient.PutAsync(
          $"/admin/realms/{_options.Realm}/roles/{roleName}",
          content,
          cancellationToken);

      if (!response.IsSuccessStatusCode)
      {
        _logger.LogError("Błąd podczas aktualizacji roli w Keycloak. Status: {StatusCode}, Treść: {Content}",
            response.StatusCode, await response.Content.ReadAsStringAsync(cancellationToken));
      }

      response.EnsureSuccessStatusCode();

      _logger.LogInformation("Zaktualizowano rolę {RoleName} w Keycloak", roleName);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Wystąpił błąd podczas aktualizacji roli {RoleName} w Keycloak", roleName);
      throw;
    }
  }

  /// <inheritdoc />
  public async Task DeleteRoleAsync(string roleName, CancellationToken cancellationToken = default)
  {
    try
    {
      var token = await GetAdminAccessTokenAsync(cancellationToken);
      _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

      var response = await _httpClient.DeleteAsync(
          $"/admin/realms/{_options.Realm}/roles/{roleName}",
          cancellationToken);

      if (!response.IsSuccessStatusCode)
      {
        _logger.LogError("Błąd podczas usuwania roli z Keycloak. Status: {StatusCode}, Treść: {Content}",
            response.StatusCode, await response.Content.ReadAsStringAsync(cancellationToken));
      }

      response.EnsureSuccessStatusCode();

      _logger.LogInformation("Usunięto rolę {RoleName} z Keycloak", roleName);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Wystąpił błąd podczas usuwania roli {RoleName} z Keycloak", roleName);
      throw;
    }
  }

  /// <inheritdoc />
  public async Task<List<KeycloakUser>> GetAllUsersAsync(CancellationToken cancellationToken = default)
  {
    try
    {
      var token = await GetAdminAccessTokenAsync(cancellationToken);
      _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

      var response = await _httpClient.GetAsync($"/admin/realms/{_options.Realm}/users?max=1000", cancellationToken);

      if (response.IsSuccessStatusCode)
      {
        return await response.Content.ReadFromJsonAsync<List<KeycloakUser>>(cancellationToken: cancellationToken) ?? new List<KeycloakUser>();
      }

      _logger.LogError("Błąd podczas pobierania wszystkich użytkowników z Keycloak. Status: {StatusCode}, Treść: {Content}",
          response.StatusCode, await response.Content.ReadAsStringAsync(cancellationToken));

      response.EnsureSuccessStatusCode();
      return new List<KeycloakUser>();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Wystąpił błąd podczas pobierania wszystkich użytkowników z Keycloak");
      throw;
    }
  }

  /// <inheritdoc />
  public async Task<List<KeycloakUser>> GetUsersByIdsAsync(List<string> userIds)
  {
    var users = new List<KeycloakUser>();
    foreach (var userId in userIds)
    {
      var user = await GetUserByIdAsync(userId);
      if (user != null)
      {
        users.Add(user);
      }
    }
    return users;
  }

  /// <inheritdoc />
  public async Task UpdateUserEnabledStatusAsync(string userId, bool isEnabled, CancellationToken cancellationToken = default)
  {
    try
    {
      var user = await GetUserByIdAsync(userId);
      if (user == null)
      {
        return;
      }

      var updateUserRequest = new
      {
        enabled = isEnabled
      };

      var response = await _httpClient.PutAsJsonAsync($"/admin/realms/{_options.Realm}/users/{userId}", updateUserRequest, cancellationToken);
      response.EnsureSuccessStatusCode();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error updating user enabled status for user {UserId}", userId);
    }
  }
}
