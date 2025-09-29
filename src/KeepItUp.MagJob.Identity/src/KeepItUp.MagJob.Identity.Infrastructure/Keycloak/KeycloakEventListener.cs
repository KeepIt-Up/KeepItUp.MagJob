using System.Net.Http.Headers;
using System.Net.Http.Json;
using KeepItUp.MagJob.Identity.Core.Interfaces;
using KeepItUp.MagJob.Identity.Core.Keycloak;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;
using Microsoft.Extensions.Hosting;

namespace KeepItUp.MagJob.Identity.Infrastructure.Keycloak;

/// <summary>
/// Service listening for events from Keycloak
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
    /// Initializes a new instance of the <see cref="KeycloakEventListener"/> class.
    /// </summary>
    /// <param name="keycloakClient">Keycloak client</param>
    /// <param name="logger">Logger</param>
    /// <param name="keycloakOptions">Keycloak options</param>
    /// <param name="httpClientFactory">HTTP client factory</param>
    /// <param name="serviceScopeFactory">Service scope factory</param>
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

                // Wait before checking for events again
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Normal termination
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas przetwarzania zdarzeń Keycloak");

                // Wait before trying again
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
            _logger.LogDebug("Pobieranie tokenu klienta usługi Keycloak");
            var token = await _keycloakClient.GetAdminAccessTokenAsync(cancellationToken);

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogError("Nie udało się pobrać tokenu klienta usługi Keycloak");
                return;
            }

            _logger.LogDebug("Token klienta usługi Keycloak pobrany pomyślnie");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Get events from the last check
            // Convert the date to the yyyy-MM-dd format required by Keycloak
            var fromDate = _lastEventTime.ToString("yyyy-MM-dd");

            // Endpoint for getting user events
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

                    // Try to get information about the service account to verify which roles are assigned
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

            // Update the time of the last event
            var latestEventTime = events.Max(e => e.Time);
            if (latestEventTime > 0)
            {
                _lastEventTime = DateTimeOffset.FromUnixTimeMilliseconds(latestEventTime).UtcDateTime;
            }

            // Process events
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
                // We can update the last login of the user
                await HandleUserLoginEventAsync(keycloakEvent.UserId, cancellationToken);
                break;

            case "DELETE_ACCOUNT":
                await HandleUserDeleteEventAsync(keycloakEvent.UserId, cancellationToken);
                break;

            case "UPDATE_PASSWORD":
                // We can react to the password change
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
            var user = await userRepository.GetByExternalIdAsync(Guid.Parse(userId), cancellationToken);
            if (user != null)
            {
                user.UpdateLastLoginDate(DateTime.UtcNow);

                try
                {
                    await userRepository.UpdateAsync(user, cancellationToken);
                    _logger.LogInformation("Zaktualizowano datę ostatniego logowania dla użytkownika {UserId}", userId);
                }
                catch (KeepItUp.MagJob.Identity.Core.Exceptions.ConcurrencyException)
                {
                    // In case of a concurrency conflict, we can try again
                    _logger.LogWarning("Wystąpił konflikt współbieżności podczas aktualizacji daty logowania dla użytkownika {UserId}, ignorowanie", userId);
                    // Ignore the concurrency error - the login date is not a critical information
                }
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
            var user = await userRepository.GetByExternalIdAsync(Guid.Parse(userId), cancellationToken);
            if (user != null)
            {
                // We can mark the user as inactive instead of deleting them
                user.Deactivate();

                try
                {
                    await userRepository.UpdateAsync(user, cancellationToken);
                    _logger.LogInformation("Użytkownik {UserId} został dezaktywowany po usunięciu konta w Keycloak", userId);
                }
                catch (KeepItUp.MagJob.Identity.Core.Exceptions.ConcurrencyException ex)
                {
                    // In case of a concurrency conflict, read the user again and try again
                    _logger.LogWarning(ex, "Wystąpił konflikt współbieżności podczas dezaktywacji użytkownika {UserId}, próba ponowna", userId);

                    // Read the user again and try to update
                    var refreshedUser = await userRepository.GetByExternalIdAsync(Guid.Parse(userId), cancellationToken);
                    if (refreshedUser != null && refreshedUser.IsActive)
                    {
                        refreshedUser.Deactivate();
                        await userRepository.UpdateAsync(refreshedUser, cancellationToken);
                        _logger.LogInformation("Użytkownik {UserId} został dezaktywowany po ponownej próbie", userId);
                    }
                }
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

            // Check if the user already exists in our database
            var existingUser = await userRepository.GetByExternalIdAsync(Guid.Parse(userId), cancellationToken);

            if (existingUser != null)
            {
                // Update the existing user
                await keycloakSyncService.SyncUserDataAsync(userId, cancellationToken);
                _logger.LogInformation("Zaktualizowano dane użytkownika {UserId} z Keycloak", userId);
            }
            else
            {
                // Import the new user
                await keycloakSyncService.SyncUserDataAsync(userId, cancellationToken);
                _logger.LogInformation("Zaimportowano nowego użytkownika {UserId} z Keycloak", userId);
            }

            // Synchronize the user's roles
            await keycloakSyncService.SyncUserRolesAsync(userId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas obsługi zdarzenia rejestracji użytkownika {UserId}", userId);
            throw;
        }
    }
}
