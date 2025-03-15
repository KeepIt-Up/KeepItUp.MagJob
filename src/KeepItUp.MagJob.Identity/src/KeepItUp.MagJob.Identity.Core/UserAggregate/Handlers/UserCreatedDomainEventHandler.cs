using KeepItUp.MagJob.Identity.Core.UserAggregate.Events;
using KeepItUp.MagJob.Identity.Core.Interfaces;


namespace KeepItUp.MagJob.Identity.Core.UserAggregate.Handlers;

/// <summary>
/// Handler dla zdarzenia utworzenia użytkownika
/// </summary>
internal class UserCreatedDomainEventHandler : INotificationHandler<UserCreatedEvent>
{
  private readonly IKeycloakSyncService _keycloakSyncService;
  private readonly IRepository<User> _userRepository;
  private readonly ILogger<UserCreatedDomainEventHandler> _logger;

  /// <summary>
  /// Inicjalizuje nową instancję klasy <see cref="UserCreatedDomainEventHandler"/>
  /// </summary>
  /// <param name="keycloakSyncService">Serwis synchronizacji z Keycloak</param>
  /// <param name="userRepository">Repozytorium użytkowników</param>
  /// <param name="logger">Logger</param>
  public UserCreatedDomainEventHandler(
      IKeycloakSyncService keycloakSyncService,
      IRepository<User> userRepository,
      ILogger<UserCreatedDomainEventHandler> logger)
  {
    _keycloakSyncService = keycloakSyncService ?? throw new ArgumentNullException(nameof(keycloakSyncService));
    _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }

  /// <summary>
  /// Obsługuje zdarzenie utworzenia użytkownika
  /// </summary>
  /// <param name="notification">Zdarzenie</param>
  /// <param name="cancellationToken">Token anulowania</param>
  /// <returns>Task reprezentujący asynchroniczną operację</returns>
  public async Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
  {
    try
    {
      _logger.LogInformation("Obsługa zdarzenia utworzenia użytkownika {UserId}", notification.UserId);

      // Jeśli użytkownik został utworzony w naszym systemie, ale nie ma jeszcze konta w Keycloak,
      // możemy utworzyć je automatycznie
      if (string.IsNullOrEmpty(notification.ExternalId))
      {
        var user = await _userRepository.GetByIdAsync(notification.UserId, cancellationToken);
        if (user == null)
        {
          _logger.LogWarning("Nie znaleziono użytkownika o identyfikatorze {UserId}", notification.UserId);
          return;
        }

        // Tutaj można zaimplementować logikę tworzenia użytkownika w Keycloak
        // Na razie tylko logujemy informację
        _logger.LogInformation("Użytkownik {UserId} został utworzony w systemie, ale nie ma jeszcze konta w Keycloak", notification.UserId);
      }

      _logger.LogInformation("Zakończono obsługę zdarzenia utworzenia użytkownika {UserId}", notification.UserId);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Wystąpił błąd podczas obsługi zdarzenia utworzenia użytkownika {UserId}", notification.UserId);
      throw;
    }
  }
}
