using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using KeepItUp.MagJob.Identity.Infrastructure.Keycloak.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.Infrastructure.Keycloak;

/// <summary>
/// Serwis nasłuchujący zdarzeń z Keycloak
/// </summary>
public class KeycloakEventListener : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<KeycloakEventListener> _logger;
    
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="KeycloakEventListener"/>
    /// </summary>
    /// <param name="serviceProvider">Dostawca usług</param>
    /// <param name="logger">Logger</param>
    public KeycloakEventListener(
        IServiceProvider serviceProvider,
        ILogger<KeycloakEventListener> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Uruchomiono nasłuchiwanie zdarzeń z Keycloak");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // W rzeczywistej implementacji tutaj byłoby nasłuchiwanie zdarzeń z Keycloak
                // np. poprzez webhook lub kolejkę wiadomości
                
                // Symulacja oczekiwania na zdarzenia
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Ignoruj wyjątek anulowania operacji
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Wystąpił błąd podczas nasłuchiwania zdarzeń z Keycloak");
                
                // Poczekaj przed ponowną próbą
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
        
        _logger.LogInformation("Zatrzymano nasłuchiwanie zdarzeń z Keycloak");
    }
    
    /// <summary>
    /// Obsługuje zdarzenie rejestracji użytkownika
    /// </summary>
    /// <param name="eventData">Dane zdarzenia</param>
    /// <returns>Task reprezentujący asynchroniczną operację</returns>
    public async Task HandleUserRegistrationEventAsync(string eventData)
    {
        try
        {
            var keycloakEvent = JsonSerializer.Deserialize<KeycloakEvent>(eventData);
            if (keycloakEvent == null || keycloakEvent.Type != "REGISTER" || string.IsNullOrEmpty(keycloakEvent.UserId))
            {
                _logger.LogWarning("Otrzymano nieprawidłowe dane zdarzenia rejestracji użytkownika");
                return;
            }
            
            using var scope = _serviceProvider.CreateScope();
            var keycloakSyncService = scope.ServiceProvider.GetRequiredService<IKeycloakSyncService>();
            
            // Importuj użytkownika z Keycloak do modułu Identity
            await keycloakSyncService.ImportUserFromKeycloakAsync(keycloakEvent.UserId);
            
            _logger.LogInformation("Zaimportowano nowego użytkownika {UserId} z Keycloak", keycloakEvent.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Wystąpił błąd podczas obsługi zdarzenia rejestracji użytkownika");
        }
    }
    
    /// <summary>
    /// Obsługuje zdarzenie aktualizacji użytkownika
    /// </summary>
    /// <param name="eventData">Dane zdarzenia</param>
    /// <returns>Task reprezentujący asynchroniczną operację</returns>
    public async Task HandleUserUpdateEventAsync(string eventData)
    {
        try
        {
            var keycloakEvent = JsonSerializer.Deserialize<KeycloakEvent>(eventData);
            if (keycloakEvent == null || keycloakEvent.Type != "UPDATE_PROFILE" || string.IsNullOrEmpty(keycloakEvent.UserId))
            {
                _logger.LogWarning("Otrzymano nieprawidłowe dane zdarzenia aktualizacji użytkownika");
                return;
            }
            
            using var scope = _serviceProvider.CreateScope();
            var keycloakClient = scope.ServiceProvider.GetRequiredService<IKeycloakClient>();
            var userRepository = scope.ServiceProvider.GetRequiredService<IRepository<Core.UserAggregate.User>>();
            
            // Pobierz użytkownika z modułu Identity na podstawie identyfikatora zewnętrznego
            var user = await userRepository.FirstOrDefaultAsync(
                new UserByExternalIdSpecification(keycloakEvent.UserId));
            
            if (user == null)
            {
                _logger.LogWarning("Nie znaleziono użytkownika o identyfikatorze zewnętrznym {ExternalId}", keycloakEvent.UserId);
                return;
            }
            
            // Pobierz zaktualizowane dane użytkownika z Keycloak
            var keycloakUser = await keycloakClient.GetUserByIdAsync(keycloakEvent.UserId);
            if (keycloakUser == null)
            {
                _logger.LogWarning("Nie znaleziono użytkownika o identyfikatorze {UserId} w Keycloak", keycloakEvent.UserId);
                return;
            }
            
            // Aktualizuj dane użytkownika w module Identity
            user.UpdateAllDetails(
                keycloakUser.FirstName ?? string.Empty,
                keycloakUser.LastName ?? string.Empty,
                keycloakUser.Email,
                keycloakUser.Enabled
            );
            
            await userRepository.UpdateAsync(user);
            
            _logger.LogInformation("Zaktualizowano dane użytkownika {UserId} na podstawie zdarzenia z Keycloak", user.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Wystąpił błąd podczas obsługi zdarzenia aktualizacji użytkownika");
        }
    }
}

/// <summary>
/// Reprezentuje zdarzenie z Keycloak
/// </summary>
public class KeycloakEvent
{
    /// <summary>
    /// Typ zdarzenia
    /// </summary>
    public string Type { get; set; } = string.Empty;
    
    /// <summary>
    /// Identyfikator użytkownika
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// Czas zdarzenia
    /// </summary>
    public long Time { get; set; }
    
    /// <summary>
    /// Identyfikator klienta
    /// </summary>
    public string ClientId { get; set; } = string.Empty;
    
    /// <summary>
    /// Adres IP
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;
    
    /// <summary>
    /// Szczegóły zdarzenia
    /// </summary>
    public JsonElement Details { get; set; }
} 
