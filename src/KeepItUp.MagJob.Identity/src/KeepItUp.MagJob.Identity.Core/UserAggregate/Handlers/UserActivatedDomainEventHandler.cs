using KeepItUp.MagJob.Identity.Core.UserAggregate.Events;
using KeepItUp.MagJob.Identity.Core.Interfaces;


namespace KeepItUp.MagJob.Identity.Core.UserAggregate.Handlers;

/// <summary>
/// Handler dla zdarzenia aktywacji użytkownika
/// </summary>
internal class UserActivatedDomainEventHandler : INotificationHandler<UserActivatedEvent>
{
  private readonly IKeycloakClient _keycloakClient;
  private readonly ILogger<UserActivatedDomainEventHandler> _logger;

  /// <summary>
  /// Inicjalizuje nową instancję klasy <see cref="UserActivatedDomainEventHandler"/>
  /// </summary>
  /// <param name="keycloakClient">Klient Keycloak</param>
  /// <param name="logger">Logger</param>
  public UserActivatedDomainEventHandler(
      IKeycloakClient keycloakClient,
      ILogger<UserActivatedDomainEventHandler> logger)
  {
    _keycloakClient = keycloakClient ?? throw new ArgumentNullException(nameof(keycloakClient));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }

  /// <summary>
  /// Obsługuje zdarzenie aktywacji użytkownika
  /// </summary>
  /// <param name="notification">Zdarzenie</param>
  /// <param name="cancellationToken">Token anulowania</param>
  /// <returns>Task reprezentujący asynchroniczną operację</returns>
  public async Task Handle(UserActivatedEvent notification, CancellationToken cancellationToken)
  {
    try
    {
      _logger.LogInformation("Obsługa zdarzenia aktywacji użytkownika {UserId}", notification.UserId);

      // Jeśli użytkownik ma identyfikator zewnętrzny, aktywujemy go w Keycloak
      if (!string.IsNullOrEmpty(notification.ExternalId))
      {
        await _keycloakClient.UpdateUserEnabledStatusAsync(notification.ExternalId, true, cancellationToken);
        _logger.LogInformation("Aktywowano użytkownika {UserId} w Keycloak", notification.UserId);
      }

      _logger.LogInformation("Zakończono obsługę zdarzenia aktywacji użytkownika {UserId}", notification.UserId);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Wystąpił błąd podczas obsługi zdarzenia aktywacji użytkownika {UserId}", notification.UserId);
      throw;
    }
  }
}
