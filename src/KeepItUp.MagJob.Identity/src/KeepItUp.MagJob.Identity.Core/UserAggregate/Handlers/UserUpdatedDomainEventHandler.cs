using KeepItUp.MagJob.Identity.Core.UserAggregate.Events;
using KeepItUp.MagJob.Identity.Core.Interfaces;

namespace KeepItUp.MagJob.Identity.Core.UserAggregate.Handlers;

/// <summary>
/// Handler dla zdarzenia aktualizacji użytkownika
/// </summary>
internal class UserUpdatedDomainEventHandler : INotificationHandler<UserUpdatedEvent>
{
  private readonly IKeycloakSyncService _keycloakSyncService;
  private readonly IRepository<User> _userRepository;
  private readonly ILogger<UserUpdatedDomainEventHandler> _logger;


  /// <summary>
  /// Inicjalizuje nową instancję klasy <see cref="UserUpdatedDomainEventHandler"/>
  /// </summary>
  /// <param name="keycloakSyncService">Serwis synchronizacji z Keycloak</param>
  /// <param name="userRepository">Repozytorium użytkowników</param>
  /// <param name="logger">Logger</param>
  public UserUpdatedDomainEventHandler(
      IKeycloakSyncService keycloakSyncService,
      IRepository<User> userRepository,
      ILogger<UserUpdatedDomainEventHandler> logger)
  {
    _keycloakSyncService = keycloakSyncService ?? throw new ArgumentNullException(nameof(keycloakSyncService));
    _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }

  /// <summary>
  /// Obsługuje zdarzenie aktualizacji użytkownika
  /// </summary>
  /// <param name="notification">Zdarzenie</param>
  /// <param name="cancellationToken">Token anulowania</param>
  /// <returns>Task reprezentujący asynchroniczną operację</returns>
  public async Task Handle(UserUpdatedEvent notification, CancellationToken cancellationToken)
  {
    try
    {
      _logger.LogInformation("Obsługa zdarzenia aktualizacji użytkownika {UserId}", notification.UserId);

      // Jeśli użytkownik ma identyfikator zewnętrzny, synchronizujemy dane z Keycloak
      if (!string.IsNullOrEmpty(notification.ExternalId))
      {
        var user = await _userRepository.GetByIdAsync(notification.UserId, cancellationToken);
        if (user == null)
        {
          _logger.LogWarning("Nie znaleziono użytkownika o identyfikatorze {UserId}", notification.UserId);
          return;
        }

        // Pobierz dane użytkownika z Keycloak
        await _keycloakSyncService.SyncUserDataAsync(notification.ExternalId, cancellationToken);

        _logger.LogInformation("Zsynchronizowano dane użytkownika {UserId} z Keycloak", notification.UserId);
      }

      _logger.LogInformation("Zakończono obsługę zdarzenia aktualizacji użytkownika {UserId}", notification.UserId);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Wystąpił błąd podczas obsługi zdarzenia aktualizacji użytkownika {UserId}", notification.UserId);
      throw;
    }
  }
}
