using KeepItUp.MagJob.Identity.Core.Interfaces;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Events;


namespace KeepItUp.MagJob.Identity.Core.UserAggregate.Handlers;

/// <summary>
/// Handler for the event of deactivating a user
/// </summary>
internal class UserDeactivatedDomainEventHandler : INotificationHandler<UserDeactivatedEvent>
{
    private readonly IKeycloakClient _keycloakClient;
    private readonly ILogger<UserDeactivatedDomainEventHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserDeactivatedDomainEventHandler"/> class
    /// </summary>
    /// <param name="keycloakClient">Keycloak client</param>
    /// <param name="logger">Logger</param>
    public UserDeactivatedDomainEventHandler(
        IKeycloakClient keycloakClient,
        ILogger<UserDeactivatedDomainEventHandler> logger)
    {
        _keycloakClient = keycloakClient ?? throw new ArgumentNullException(nameof(keycloakClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the event of deactivating a user
    /// </summary>
    /// <param name="notification">Event</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing an asynchronous operation</returns>
    public async Task Handle(UserDeactivatedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Obsługa zdarzenia dezaktywacji użytkownika {UserId}", notification.UserId);

            if (notification.ExternalId != Guid.Empty)
            {
                await _keycloakClient.UpdateUserEnabledStatusAsync(notification.ExternalId.ToString(), false, cancellationToken);
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
