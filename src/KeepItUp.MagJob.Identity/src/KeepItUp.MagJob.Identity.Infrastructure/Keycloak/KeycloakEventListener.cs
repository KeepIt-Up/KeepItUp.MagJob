using System.Net.Http.Headers;
using System.Net.Http.Json;
using KeepItUp.MagJob.Identity.Core.Interfaces;
using KeepItUp.MagJob.Identity.Core.Keycloak;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;
using Microsoft.Extensions.Hosting;

namespace KeepItUp.MagJob.Identity.Infrastructure.Keycloak;

/// <summary>
/// Serwis nasłuchujący zdarzeń z Keycloak
/// </summary>
public class KeycloakEventListener : BackgroundService
{
    private readonly IKeycloakClient _keycloakClient;
    private readonly ILogger<KeycloakEventListener> _logger;
    private readonly KeycloakAdminOptions _keycloakOptions;
    private readonly HttpClient _httpClient;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private DateTime _lastEventTime = DateTime.UtcNow.AddMinutes(-5); // Start by fetching events from 5 minutes ago

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="KeycloakEventListener"/>
    /// </summary>
    /// <param name="keycloakClient">Klient Keycloak</param>
    /// <param name="logger">Logger</param>
    /// <param name="keycloakOptions">Opcje Keycloak</param>
    /// <param name="httpClientFactory">Fabryka klientów HTTP</param>
    /// <param name="serviceScopeFactory">Fabryka zakresów usług</param>
    public KeycloakEventListener(
        IKeycloakClient keycloakClient,
        ILogger<KeycloakEventListener> logger,
        IOptions<KeycloakAdminOptions> keycloakOptions,
        IHttpClientFactory httpClientFactory,
        IServiceScopeFactory serviceScopeFactory)
    {
        _keycloakClient = keycloakClient;
        _logger = logger;
        _keycloakOptions = keycloakOptions.Value;
        _httpClient = httpClientFactory.CreateClient("KeycloakEvents");
        _httpClient.BaseAddress = new Uri(_keycloakOptions.ServerUrl);
        _serviceScopeFactory = serviceScopeFactory;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Keycloak Event Listener uruchomiony");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _semaphore.WaitAsync(stoppingToken);
                try
                {
                    await FetchAndProcessEventsAsync(stoppingToken);
                }
                finally
                {
                    _semaphore.Release();
                }

                // Poczekaj przed kolejnym sprawdzeniem zdarzeń
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Normalne zakończenie
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas przetwarzania zdarzeń Keycloak");

                // Poczekaj przed ponowną próbą
                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }

        _logger.LogInformation("Keycloak Event Listener zatrzymany");
    }

    private async Task FetchAndProcessEventsAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Pobierz token klienta usługi
            _logger.LogDebug("Pobieranie tokenu klienta usługi Keycloak");
            var token = await _keycloakClient.GetAdminAccessTokenAsync(cancellationToken);

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogError("Nie udało się pobrać tokenu klienta usługi Keycloak");
                return;
            }

            _logger.LogDebug("Token klienta usługi Keycloak pobrany pomyślnie");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Pobierz zdarzenia od ostatniego sprawdzenia
            // Konwertuj datę na format yyyy-MM-dd wymagany przez Keycloak
            var fromDate = _lastEventTime.ToString("yyyy-MM-dd");

            // Endpoint do pobierania zdarzeń użytkowników
            var eventsUrl = $"/admin/realms/{_keycloakOptions.Realm}/events?first=0&max=100&dateFrom={fromDate}";
            _logger.LogDebug("Pobieranie zdarzeń Keycloak z URL: {Url}", eventsUrl);

            var response = await _httpClient.GetAsync(eventsUrl, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Błąd podczas pobierania zdarzeń z Keycloak. Status: {StatusCode}, URL: {Url}, Treść: {Content}",
                    response.StatusCode,
                    eventsUrl,
                    responseContent);

                if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    _logger.LogWarning("Konto usługi nie ma wystarczających uprawnień do pobierania zdarzeń. " +
                                      "Upewnij się, że konto usługi ma przypisaną rolę 'view-events' w Keycloak.");

                    // Spróbuj pobrać informacje o koncie usługi, aby zweryfikować, jakie role są przypisane
                    try
                    {
                        var serviceAccountUrl = $"/admin/realms/{_keycloakOptions.Realm}/users?username=service-account-{_keycloakOptions.ClientId}";
                        var serviceAccountResponse = await _httpClient.GetAsync(serviceAccountUrl, cancellationToken);

                        if (serviceAccountResponse.IsSuccessStatusCode)
                        {
                            var users = await serviceAccountResponse.Content.ReadFromJsonAsync<List<object>>(cancellationToken: cancellationToken);
                            _logger.LogInformation("Znaleziono {Count} kont usługi dla klienta {ClientId}",
                                users?.Count ?? 0, _keycloakOptions.ClientId);
                        }
                        else
                        {
                            _logger.LogWarning("Nie można pobrać informacji o koncie usługi. Status: {StatusCode}",
                                serviceAccountResponse.StatusCode);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Błąd podczas próby pobrania informacji o koncie usługi");
                    }
                }

                return;
            }

            var events = await response.Content.ReadFromJsonAsync<List<KeycloakEvent>>(cancellationToken: cancellationToken);

            if (events == null || !events.Any())
            {
                _logger.LogDebug("Brak nowych zdarzeń z Keycloak");
                return;
            }

            _logger.LogInformation("Pobrano {Count} zdarzeń z Keycloak", events.Count);

            // Aktualizuj czas ostatniego zdarzenia
            var latestEventTime = events.Max(e => e.Time);
            if (latestEventTime > 0)
            {
                _lastEventTime = DateTimeOffset.FromUnixTimeMilliseconds(latestEventTime).UtcDateTime;
            }

            // Przetwórz zdarzenia
            foreach (var keycloakEvent in events.OrderBy(e => e.Time))
            {
                try
                {
                    await ProcessEventAsync(keycloakEvent, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Błąd podczas przetwarzania zdarzenia Keycloak: {EventType}, UserId: {UserId}",
                        keycloakEvent.Type, keycloakEvent.UserId);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania i przetwarzania zdarzeń z Keycloak");
        }
    }

    private async Task ProcessEventAsync(KeycloakEvent keycloakEvent, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Przetwarzanie zdarzenia Keycloak: {EventType}, UserId: {UserId}",
            keycloakEvent.Type, keycloakEvent.UserId);

        switch (keycloakEvent.Type)
        {
            case "REGISTER":
            case "UPDATE_PROFILE":
                await HandleUserRegistrationEventAsync(keycloakEvent.UserId, cancellationToken);
                break;

            case "LOGIN":
                // Możemy zaktualizować ostatnie logowanie użytkownika
                await HandleUserLoginEventAsync(keycloakEvent.UserId, cancellationToken);
                break;

            case "DELETE_ACCOUNT":
                await HandleUserDeleteEventAsync(keycloakEvent.UserId, cancellationToken);
                break;

            case "UPDATE_PASSWORD":
                // Możemy zareagować na zmianę hasła
                _logger.LogInformation("Użytkownik {UserId} zmienił hasło", keycloakEvent.UserId);
                break;

            case "CLIENT_ROLE_MAPPING":
            case "REALM_ROLE_MAPPING":
                await HandleRoleMappingEventAsync(keycloakEvent.UserId, cancellationToken);
                break;

            default:
                _logger.LogDebug("Nieobsługiwany typ zdarzenia: {EventType}", keycloakEvent.Type);
                break;
        }
    }

    private async Task HandleUserLoginEventAsync(string userId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("Próba obsługi zdarzenia logowania z pustym ID użytkownika");
            return;
        }

        using var scope = _serviceScopeFactory.CreateScope();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        try
        {
            var user = await userRepository.GetByExternalIdAsync(userId, cancellationToken);
            if (user != null)
            {
                user.UpdateLastLoginDate(DateTime.UtcNow);
                await userRepository.UpdateAsync(user, cancellationToken);
                _logger.LogInformation("Zaktualizowano datę ostatniego logowania dla użytkownika {UserId}", userId);
            }
            else
            {
                _logger.LogWarning("Nie znaleziono użytkownika o identyfikatorze zewnętrznym {ExternalId} podczas aktualizacji daty logowania", userId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas aktualizacji daty logowania dla użytkownika {ExternalId}", userId);
        }
    }

    private async Task HandleUserDeleteEventAsync(string userId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("Próba obsługi zdarzenia usunięcia z pustym ID użytkownika");
            return;
        }

        using var scope = _serviceScopeFactory.CreateScope();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        try
        {
            var user = await userRepository.GetByExternalIdAsync(userId, cancellationToken);
            if (user != null)
            {
                // Możemy oznaczyć użytkownika jako nieaktywnego zamiast go usuwać
                user.Deactivate();
                await userRepository.UpdateAsync(user, cancellationToken);
                _logger.LogInformation("Użytkownik {UserId} został dezaktywowany po usunięciu konta w Keycloak", userId);
            }
            else
            {
                _logger.LogWarning("Nie znaleziono użytkownika o identyfikatorze zewnętrznym {ExternalId} podczas dezaktywacji", userId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas dezaktywacji użytkownika {ExternalId}", userId);
        }
    }

    private async Task HandleRoleMappingEventAsync(string userId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("Próba obsługi zdarzenia mapowania ról z pustym ID użytkownika");
            return;
        }

        using var scope = _serviceScopeFactory.CreateScope();
        var keycloakSyncService = scope.ServiceProvider.GetRequiredService<IKeycloakSyncService>();

        await keycloakSyncService.SyncUserRolesAsync(userId, cancellationToken);
        _logger.LogInformation("Zsynchronizowano role użytkownika {UserId} po zmianie mapowania ról", userId);
    }

    private async Task HandleUserRegistrationEventAsync(string userId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("Próba obsługi zdarzenia rejestracji z pustym ID użytkownika");
            return;
        }

        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var keycloakSyncService = scope.ServiceProvider.GetRequiredService<IKeycloakSyncService>();
            var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

            // Sprawdź, czy użytkownik już istnieje w naszej bazie danych
            var existingUser = await userRepository.GetByExternalIdAsync(userId, cancellationToken);

            if (existingUser != null)
            {
                // Aktualizuj istniejącego użytkownika
                await keycloakSyncService.SyncUserDataAsync(userId, cancellationToken);
                _logger.LogInformation("Zaktualizowano dane użytkownika {UserId} z Keycloak", userId);
            }
            else
            {
                // Importuj nowego użytkownika
                await keycloakSyncService.SyncUserDataAsync(userId, cancellationToken);
                _logger.LogInformation("Zaimportowano nowego użytkownika {UserId} z Keycloak", userId);
            }

            // Synchronizuj role użytkownika
            await keycloakSyncService.SyncUserRolesAsync(userId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas obsługi zdarzenia rejestracji użytkownika {UserId}", userId);
            throw;
        }
    }
}
