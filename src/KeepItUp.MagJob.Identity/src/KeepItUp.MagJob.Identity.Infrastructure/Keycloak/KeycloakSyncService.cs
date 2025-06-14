using KeepItUp.MagJob.Identity.Core.Interfaces;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;

namespace KeepItUp.MagJob.Identity.Infrastructure.Keycloak;

/// <summary>
/// Implementation of the service for synchronizing data between the Identity module and Keycloak
/// </summary>
public class KeycloakSyncService : IKeycloakSyncService
{
    private readonly IKeycloakClient _keycloakClient;
    private readonly IUserRepository _userRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly ILogger<KeycloakSyncService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeycloakSyncService"/> class.
    /// </summary>
    /// <param name="keycloakClient">Keycloak client.</param>
    /// <param name="userRepository">User repository.</param>
    /// <param name="organizationRepository">Organization repository.</param>
    /// <param name="logger">Logger.</param>
    public KeycloakSyncService(
        IKeycloakClient keycloakClient,
        IUserRepository userRepository,
        IOrganizationRepository organizationRepository,
        ILogger<KeycloakSyncService> logger)
    {
        _keycloakClient = keycloakClient ?? throw new ArgumentNullException(nameof(keycloakClient));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _organizationRepository = organizationRepository ?? throw new ArgumentNullException(nameof(organizationRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task SyncUserRolesAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Rozpoczęto synchronizację ról użytkownika {UserId} z Keycloak", userId);

            // Get the user from our database
            var user = await _userRepository.GetByExternalIdAsync(Guid.Parse(userId), cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("Nie znaleziono użytkownika o identyfikatorze zewnętrznym {ExternalId} podczas synchronizacji ról", userId);
                return;
            }

            // Get the user's roles from Keycloak
            var keycloakRoles = await _keycloakClient.GetUserRolesAsync(userId, cancellationToken);

            // Map the Keycloak roles to permissions in our application
            var permissions = MapRolesToPermissions(keycloakRoles);

            // Update the user's permissions
            user.UpdatePermissions(permissions);
            await _userRepository.UpdateAsync(user, cancellationToken);

            // Update the user's attributes in Keycloak
            var keycloakUser = await _keycloakClient.GetUserByIdAsync(userId, cancellationToken);
            if (keycloakUser != null)
            {
                // Add the user's organizations as attributes
                var organizations = await _organizationRepository.GetByUserIdAsync(user.Id, cancellationToken);
                var organizationIds = organizations.Select(o => o.Id.ToString()).ToList();

                if (keycloakUser.Attributes == null)
                {
                    keycloakUser.Attributes = new Dictionary<string, List<string>>();
                }

                keycloakUser.Attributes["organizations"] = organizationIds;
                keycloakUser.Attributes["permissions"] = permissions;

                await _keycloakClient.UpdateUserAsync(keycloakUser, cancellationToken);
            }

            _logger.LogInformation("Zakończono synchronizację ról użytkownika {UserId} z Keycloak", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Wystąpił błąd podczas synchronizacji ról użytkownika {UserId} z Keycloak", userId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task SyncUserDataAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Rozpoczęto synchronizację danych użytkownika {UserId} z Keycloak", userId);

            // Get the user's data from Keycloak
            var keycloakUser = await _keycloakClient.GetUserByIdAsync(userId, cancellationToken);
            if (keycloakUser == null)
            {
                _logger.LogWarning("Nie znaleziono użytkownika o identyfikatorze {UserId} w Keycloak", userId);
                return;
            }

            // Check if the user already exists in our database
            var existingUser = await _userRepository.GetByExternalIdAsync(Guid.Parse(userId), cancellationToken);

            if (existingUser == null)
            {
                // Create a new user
                var newUser = User.Create(
                    keycloakUser.FirstName ?? string.Empty,
                    keycloakUser.LastName ?? string.Empty,
                    keycloakUser.Email,
                    keycloakUser.Username ?? keycloakUser.Email,
                    Guid.Parse(userId),
                    true
                );

                await _userRepository.AddAsync(newUser, cancellationToken);
                _logger.LogInformation("Utworzono nowego użytkownika {UserId} na podstawie danych z Keycloak", newUser.Id);
            }
            else
            {
                // Update the existing user
                existingUser.UpdateAllDetails(
                    keycloakUser.FirstName ?? string.Empty,
                    keycloakUser.LastName ?? string.Empty,
                    keycloakUser.Email,
                    keycloakUser.Username,
                    keycloakUser.Enabled
                );

                await _userRepository.UpdateAsync(existingUser, cancellationToken);
                _logger.LogInformation("Zaktualizowano dane użytkownika {UserId} na podstawie danych z Keycloak", existingUser.Id);
            }

            _logger.LogInformation("Zakończono synchronizację danych użytkownika {UserId} z Keycloak", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Wystąpił błąd podczas synchronizacji danych użytkownika {UserId} z Keycloak", userId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task SyncAllUsersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Rozpoczęto synchronizację wszystkich użytkowników z Keycloak");

            // Get all users from Keycloak
            var keycloakUsers = await _keycloakClient.GetAllUsersAsync(cancellationToken);

            foreach (var keycloakUser in keycloakUsers)
            {
                try
                {
                    await SyncUserDataAsync(keycloakUser.Id, cancellationToken);
                    await SyncUserRolesAsync(keycloakUser.Id, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Wystąpił błąd podczas synchronizacji użytkownika {UserId} z Keycloak", keycloakUser.Id);
                    // Continue with the synchronization of the remaining users
                }
            }

            _logger.LogInformation("Zakończono synchronizację wszystkich użytkowników z Keycloak");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Wystąpił błąd podczas synchronizacji wszystkich użytkowników z Keycloak");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<Guid> ImportUserFromKeycloakAsync(string keycloakUserId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Get the user's data from Keycloak
            var keycloakUser = await _keycloakClient.GetUserByIdAsync(keycloakUserId, cancellationToken);
            if (keycloakUser == null)
            {
                throw new InvalidOperationException($"Nie znaleziono użytkownika o identyfikatorze {keycloakUserId} w Keycloak");
            }

            // Check if the user already exists in our database
            var existingUser = await _userRepository.GetByExternalIdAsync(Guid.Parse(keycloakUserId), cancellationToken);

            if (existingUser != null)
            {
                _logger.LogInformation("Użytkownik o identyfikatorze {ExternalId} już istnieje w module Identity", keycloakUserId);
                return existingUser.Id;
            }

            // Create a new user in our database
            var newUser = User.Create(
                keycloakUser.FirstName ?? string.Empty,
                keycloakUser.LastName ?? string.Empty,
                keycloakUser.Email,
                keycloakUser.Username ?? keycloakUser.Email,
                Guid.Parse(keycloakUserId),
                true
            );

            await _userRepository.AddAsync(newUser, cancellationToken);

            _logger.LogInformation("Zaimportowano użytkownika {ExternalId} z Keycloak do modułu Identity", keycloakUserId);

            return newUser.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Wystąpił błąd podczas importowania użytkownika {ExternalId} z Keycloak", keycloakUserId);
            throw;
        }
    }

    /// <summary>
    /// Maps Keycloak roles to permissions in our application
    /// </summary>
    /// <param name="roles">Roles from Keycloak.</param>
    /// <returns>List of permissions.</returns>
    private List<string> MapRolesToPermissions(List<string> roles)
    {
        var permissions = new List<string>();

        // This is a sample implementation, which should be adapted to the actual needs
        foreach (var role in roles)
        {
            switch (role)
            {
                case "admin":
                    permissions.Add("users.view");
                    permissions.Add("users.create");
                    permissions.Add("users.edit");
                    permissions.Add("users.delete");
                    permissions.Add("organizations.view");
                    permissions.Add("organizations.create");
                    permissions.Add("organizations.edit");
                    permissions.Add("organizations.delete");
                    break;

                case "manager":
                    permissions.Add("users.view");
                    permissions.Add("organizations.view");
                    permissions.Add("organizations.create");
                    permissions.Add("organizations.edit");
                    break;

                case "user":
                    permissions.Add("users.view.self");
                    permissions.Add("organizations.view");
                    break;
            }
        }

        // Remove duplicates
        return permissions.Distinct().ToList();
    }
}

