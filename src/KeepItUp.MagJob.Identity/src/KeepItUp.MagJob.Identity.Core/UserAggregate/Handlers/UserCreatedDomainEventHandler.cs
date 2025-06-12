using KeepItUp.MagJob.Identity.Core.Interfaces;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Events;


namespace KeepItUp.MagJob.Identity.Core.UserAggregate.Handlers;

/// <summary>
/// Handler for the event of creating a user
/// </summary>
internal class UserCreatedDomainEventHandler : INotificationHandler<UserCreatedEvent>
{
    private readonly IKeycloakSyncService _keycloakSyncService;
    private readonly IRepository<User> _userRepository;
    private readonly ILogger<UserCreatedDomainEventHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserCreatedDomainEventHandler"/> class
    /// </summary>
    /// <param name="keycloakSyncService">Keycloak synchronization service</param>
    /// <param name="userRepository">User repository</param>
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
    /// Handles the event of creating a user
    /// </summary>
    /// <param name="notification">Event</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing an asynchronous operation</returns>
    public async Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Obsługa zdarzenia utworzenia użytkownika {UserId}", notification.UserId);

            if (notification.ExternalId != Guid.Empty)
            {
                var user = await _userRepository.GetByIdAsync(notification.UserId, cancellationToken);
                if (user == null)
                {
                    _logger.LogWarning("Nie znaleziono użytkownika o identyfikatorze {UserId}", notification.UserId);
                    return;
                }

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
