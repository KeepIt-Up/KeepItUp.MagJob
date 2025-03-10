using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using KeepItUp.MagJob.Identity.Infrastructure.Keycloak.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KeepItUp.MagJob.Identity.Infrastructure.Keycloak;

/// <summary>
/// Implementacja klienta do komunikacji z API Keycloak
/// </summary>
public class KeycloakClient : IKeycloakClient
{
    private readonly HttpClient _httpClient;
    private readonly KeycloakOptions _options;
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
        IOptions<KeycloakOptions> options,
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
    public async Task UpdateUserAsync(string userId, KeycloakUser user, CancellationToken cancellationToken = default)
    {
        try
        {
            var token = await GetAdminAccessTokenAsync(cancellationToken);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            var response = await _httpClient.PutAsJsonAsync($"/admin/realms/{_options.Realm}/users/{userId}", user, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Błąd podczas aktualizacji użytkownika w Keycloak. Status: {StatusCode}, Treść: {Content}",
                    response.StatusCode, await response.Content.ReadAsStringAsync(cancellationToken));
                
                response.EnsureSuccessStatusCode();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Wystąpił błąd podczas aktualizacji użytkownika w Keycloak");
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
            await UpdateUserAsync(userId, user, cancellationToken);
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
            await UpdateUserAsync(userId, user, cancellationToken);
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
            
            await UpdateUserAsync(userId, user, cancellationToken);
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
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", _options.ClientId),
                new KeyValuePair<string, string>("client_secret", _options.ClientSecret),
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            });
            
            var response = await _httpClient.PostAsync($"/realms/{_options.Realm}/protocol/openid-connect/token", content, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
                var accessToken = tokenResponse.GetProperty("access_token").GetString();
                var expiresIn = tokenResponse.GetProperty("expires_in").GetInt32();
                
                if (string.IsNullOrEmpty(accessToken))
                {
                    throw new InvalidOperationException("Otrzymano pusty token dostępu z Keycloak");
                }
                
                // Zapisz token i czas jego wygaśnięcia
                _cachedToken = accessToken;
                _tokenExpiration = DateTime.UtcNow.AddSeconds(expiresIn - 30); // Odejmij 30 sekund dla bezpieczeństwa
                
                return accessToken;
            }
            
            _logger.LogError("Błąd podczas pobierania tokenu dostępu z Keycloak. Status: {StatusCode}, Treść: {Content}",
                response.StatusCode, await response.Content.ReadAsStringAsync(cancellationToken));
            
            response.EnsureSuccessStatusCode();
            return string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Wystąpił błąd podczas pobierania tokenu dostępu z Keycloak");
            throw;
        }
    }
} 
