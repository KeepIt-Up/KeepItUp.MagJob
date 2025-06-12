using KeepItUp.MagJob.Identity.Core.Interfaces;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Events;

namespace KeepItUp.MagJob.Identity.Core.UserAggregate.Handlers;

/// <summary>
/// Handler for the event of updating a user
/// </summary>
internal class UserUpdatedDomainEventHandler : INotificationHandler<UserUpdatedEvent>
{
    private readonly IKeycloakSyncService _keycloakSyncService;
    private readonly IRepository<User> _userRepository;
    private readonly ILogger<UserUpdatedDomainEventHandler> _logger;


    /// <summary>
    /// Initializes a new instance of the <see cref="UserUpdatedDomainEventHandler"/> class
    /// </summary>
    /// <param name="keycloakSyncService">Keycloak synchronization service</param>
    /// <param name="userRepository">User repository</param>
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
    /// Handles the event of updating a user
    /// </summary>
    /// <param name="notification">Event</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing an asynchronous operation</returns>
    public async Task Handle(UserUpdatedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Obsługa zdarzenia aktualizacji użytkownika {UserId}", notification.UserId);

            if (notification.ExternalId != Guid.Empty)
            {
                var user = await _userRepository.GetByIdAsync(notification.UserId, cancellationToken);
                if (user == null)
                {
                    _logger.LogWarning("Nie znaleziono użytkownika o identyfikatorze {UserId}", notification.UserId);
                    return;
                }

                await _keycloakSyncService.SyncUserDataAsync(notification.ExternalId.ToString(), cancellationToken);

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
