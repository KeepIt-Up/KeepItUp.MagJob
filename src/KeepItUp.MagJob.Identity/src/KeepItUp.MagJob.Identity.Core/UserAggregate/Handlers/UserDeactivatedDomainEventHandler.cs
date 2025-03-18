using KeepItUp.MagJob.Identity.Core.Interfaces;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Events;


namespace KeepItUp.MagJob.Identity.Core.UserAggregate.Handlers;

/// <summary>
/// Handler dla zdarzenia dezaktywacji użytkownika
/// </summary>
internal class UserDeactivatedDomainEventHandler : INotificationHandler<UserDeactivatedEvent>
{
    private readonly IKeycloakClient _keycloakClient;
    private readonly ILogger<UserDeactivatedDomainEventHandler> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="UserDeactivatedDomainEventHandler"/>
    /// </summary>
    /// <param name="keycloakClient">Klient Keycloak</param>
    /// <param name="logger">Logger</param>
    public UserDeactivatedDomainEventHandler(
        IKeycloakClient keycloakClient,
        ILogger<UserDeactivatedDomainEventHandler> logger)
    {
        _keycloakClient = keycloakClient ?? throw new ArgumentNullException(nameof(keycloakClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Obsługuje zdarzenie dezaktywacji użytkownika
    /// </summary>
    /// <param name="notification">Zdarzenie</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Task reprezentujący asynchroniczną operację</returns>
    public async Task Handle(UserDeactivatedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Obsługa zdarzenia dezaktywacji użytkownika {UserId}", notification.UserId);

            // Jeśli użytkownik ma identyfikator zewnętrzny, dezaktywujemy go w Keycloak
            if (!string.IsNullOrEmpty(notification.ExternalId))
            {
                await _keycloakClient.UpdateUserEnabledStatusAsync(notification.ExternalId, false, cancellationToken);
                _logger.LogInformation("Dezaktywowano użytkownika {UserId} w Keycloak", notification.UserId);
            }

            _logger.LogInformation("Zakończono obsługę zdarzenia dezaktywacji użytkownika {UserId}", notification.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Wystąpił błąd podczas obsługi zdarzenia dezaktywacji użytkownika {UserId}", notification.UserId);
            throw;
        }
    }
}
