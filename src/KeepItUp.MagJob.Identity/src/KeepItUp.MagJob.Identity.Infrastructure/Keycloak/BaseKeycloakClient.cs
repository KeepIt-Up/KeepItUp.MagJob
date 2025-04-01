using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace KeepItUp.MagJob.Identity.Infrastructure.Keycloak;

/// <summary>
/// Bazowa klasa dla klientów Keycloak, obsługująca uwierzytelnianie i wspólne funkcjonalności.
/// </summary>
public abstract class BaseKeycloakClient
{
    /// <summary>
    /// Klient HTTP do komunikacji z API Keycloak.
    /// </summary>
    protected readonly HttpClient HttpClient;

    /// <summary>
    /// Opcje konfiguracji Keycloak.
    /// </summary>
    protected readonly KeycloakAdminOptions Options;

    /// <summary>
    /// Logger.
    /// </summary>
    protected readonly ILogger Logger;

    /// <summary>
    /// Zapisany token dostępu.
    /// </summary>
    private string? _cachedToken;

    /// <summary>
    /// Data wygaśnięcia tokenu dostępu.
    /// </summary>
    private DateTime _tokenExpiration = DateTime.MinValue;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="BaseKeycloakClient"/>.
    /// </summary>
    /// <param name="httpClient">Klient HTTP.</param>
    /// <param name="options">Opcje konfiguracji Keycloak.</param>
    /// <param name="logger">Logger.</param>
    protected BaseKeycloakClient(
        HttpClient httpClient,
        IOptions<KeycloakAdminOptions> options,
        ILogger logger)
    {
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        Options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));

        HttpClient.BaseAddress = new Uri(Options.ServerUrl);
        HttpClient.Timeout = TimeSpan.FromSeconds(Options.TimeoutSeconds);
    }

    /// <summary>
    /// Pobiera token dostępu do API administratora Keycloak.
    /// </summary>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Token dostępu.</returns>
    public async Task<string> GetAdminAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(_cachedToken) && _tokenExpiration > DateTime.UtcNow)
        {
            return _cachedToken;
        }

        try
        {
            Logger.LogDebug("Próba uwierzytelnienia jako klient usługi Keycloak: {ClientId}", Options.ClientId);

            var clientContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", Options.ClientId),
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            });

            var clientResponse = await HttpClient.PostAsync($"/realms/{Options.Realm}/protocol/openid-connect/token", clientContent, cancellationToken);

            if (clientResponse.IsSuccessStatusCode)
            {
                var tokenResponse = await clientResponse.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
                var accessToken = tokenResponse.GetProperty("access_token").GetString();
                var expiresIn = tokenResponse.GetProperty("expires_in").GetInt32();

                if (string.IsNullOrEmpty(accessToken))
                {
                    throw new InvalidOperationException("Otrzymano pusty token dostępu z Keycloak");
                }

                Logger.LogDebug("Pomyślnie pobrano token klienta usługi Keycloak");

                _cachedToken = accessToken;
                _tokenExpiration = DateTime.UtcNow.AddSeconds(expiresIn - 30);

                return accessToken;
            }

            Logger.LogError("Błąd podczas pobierania tokenu dostępu z Keycloak. Status: {StatusCode}, Treść: {Content}",
                clientResponse.StatusCode, await clientResponse.Content.ReadAsStringAsync(cancellationToken));

            clientResponse.EnsureSuccessStatusCode();
            return string.Empty;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Wystąpił błąd podczas pobierania tokenu dostępu z Keycloak");
            throw;
        }
    }

    /// <summary>
    /// Konfiguruje nagłówek Authorization dla żądania.
    /// </summary>
    /// <param name="cancellationToken">Token anulowania.</param>
    protected async Task SetAuthorizationHeaderAsync(CancellationToken cancellationToken = default)
    {
        var token = await GetAdminAccessTokenAsync(cancellationToken);
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    /// <summary>
    /// Obsługuje odpowiedź HTTP, sprawdzając jej status i deserializując zawartość.
    /// </summary>
    /// <typeparam name="T">Typ do którego ma być zdeserializowana odpowiedź.</typeparam>
    /// <param name="response">Odpowiedź HTTP.</param>
    /// <param name="errorMessage">Komunikat błędu w przypadku niepowodzenia.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Zdeserializowany obiekt lub default(T) w przypadku błędu.</returns>
    protected async Task<T?> HandleResponseAsync<T>(HttpResponseMessage response, string errorMessage, CancellationToken cancellationToken = default)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            Logger.LogError("{ErrorMessage}. Status: {StatusCode}. Treść: {ErrorContent}",
                errorMessage, response.StatusCode, errorContent);
            return default;
        }

        if (response.Content.Headers.ContentLength == 0)
        {
            return default;
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        try
        {
            return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (JsonException ex)
        {
            Logger.LogError(ex, "Błąd deserializacji odpowiedzi: {Message}. Treść: {Content}", ex.Message, content);
            return default;
        }
    }
}
