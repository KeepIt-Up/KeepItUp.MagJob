using KeepItUp.MagJob.Identity.Core.Interfaces;
using KeepItUp.MagJob.Identity.Core.Keycloak;

namespace KeepItUp.MagJob.Identity.Infrastructure.Keycloak;

/// <summary>
/// Implementation of the client for communication with the Keycloak API
/// </summary>
public class KeycloakClient : IKeycloakClient
{
    private readonly KeycloakUserClient _userClient;
    private readonly KeycloakRoleClient _roleClient;
    private readonly ILogger<KeycloakClient> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeycloakClient"/> class.
    /// </summary>
    /// <param name="httpClient">HTTP client</param>
    /// <param name="options">Keycloak configuration options</param>
    /// <param name="logger">Logger</param>
    public KeycloakClient(
        HttpClient httpClient,
        IOptions<KeycloakAdminOptions> options,
        ILogger<KeycloakClient> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Initialize clients
        _userClient = new KeycloakUserClient(
            httpClient ?? throw new ArgumentNullException(nameof(httpClient)),
            options ?? throw new ArgumentNullException(nameof(options)),
            logger);

        _roleClient = new KeycloakRoleClient(
            httpClient,
            options,
            logger);
    }

    /// <inheritdoc />
    public async Task<KeycloakUser?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _userClient.GetByIdAsync(userId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<KeycloakUser?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _userClient.GetByEmailAsync(email, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<KeycloakUser>> GetUsersAsync(string? search = null, int first = 0, int max = 100, CancellationToken cancellationToken = default)
    {
        return await _userClient.GetUsersAsync(search, first, max, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<string> CreateUserAsync(KeycloakUser user, CancellationToken cancellationToken = default)
    {
        var userId = await _userClient.CreateUserAsync(user, cancellationToken);
        if (userId == null)
        {
            throw new InvalidOperationException($"Nie udało się utworzyć użytkownika {user.Username}");
        }
        return userId;
    }

    /// <inheritdoc />
    public async Task UpdateUserAsync(KeycloakUser user, CancellationToken cancellationToken = default)
    {
        var result = await _userClient.UpdateUserAsync(user, cancellationToken);
        if (!result)
        {
            throw new InvalidOperationException($"Nie udało się zaktualizować użytkownika o ID {user.Id}");
        }
    }

    /// <inheritdoc />
    public async Task UpdateUserEnabledStatusAsync(string userId, bool enabled, CancellationToken cancellationToken = default)
    {
        var result = await _userClient.UpdateUserEnabledStatusAsync(userId, enabled, cancellationToken);
        if (!result)
        {
            throw new InvalidOperationException($"Nie udało się zaktualizować statusu użytkownika o ID {userId}");
        }
    }

    /// <inheritdoc />
    public async Task DeactivateUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var result = await _userClient.DeactivateUserAsync(userId, cancellationToken);
        if (!result)
        {
            throw new InvalidOperationException($"Nie udało się dezaktywować użytkownika o ID {userId}");
        }
    }

    /// <inheritdoc />
    public async Task ActivateUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var result = await _userClient.ActivateUserAsync(userId, cancellationToken);
        if (!result)
        {
            throw new InvalidOperationException($"Nie udało się aktywować użytkownika o ID {userId}");
        }
    }

    /// <inheritdoc />
    public async Task UpdateUserAttributesAsync(string userId, Dictionary<string, List<string>> attributes, CancellationToken cancellationToken = default)
    {
        var result = await _userClient.UpdateUserAttributesAsync(userId, attributes, cancellationToken);
        if (!result)
        {
            throw new InvalidOperationException($"Nie udało się zaktualizować atrybutów użytkownika o ID {userId}");
        }
    }

    /// <inheritdoc />
    public async Task<string> GetAdminAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        // We use the user client token, but we could also use the role client
        return await _userClient.GetAdminAccessTokenAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<string>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _roleClient.GetUserRolesAsync(userId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AssignRoleToUserAsync(string userId, string roleName, CancellationToken cancellationToken = default)
    {
        var result = await _roleClient.AssignRoleToUserAsync(userId, roleName, cancellationToken);
        if (!result)
        {
            throw new InvalidOperationException($"Nie udało się przypisać roli {roleName} do użytkownika o ID {userId}");
        }
    }

    /// <inheritdoc />
    public async Task RemoveRoleFromUserAsync(string userId, string roleName, CancellationToken cancellationToken = default)
    {
        var result = await _roleClient.RemoveRoleFromUserAsync(userId, roleName, cancellationToken);
        if (!result)
        {
            throw new InvalidOperationException($"Nie udało się usunąć roli {roleName} użytkownika o ID {userId}");
        }
    }

    /// <inheritdoc />
    public async Task<List<KeycloakRole>> GetRolesAsync(CancellationToken cancellationToken = default)
    {
        return await _roleClient.GetRolesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<string> CreateRoleAsync(KeycloakRole role, CancellationToken cancellationToken = default)
    {
        var roleId = await _roleClient.CreateRoleAsync(role, cancellationToken);
        if (roleId == null)
        {
            throw new InvalidOperationException($"Nie udało się utworzyć roli {role.Name}");
        }
        return roleId;
    }

    /// <inheritdoc />
    public async Task UpdateRoleAsync(string roleName, KeycloakRole role, CancellationToken cancellationToken = default)
    {
        var result = await _roleClient.UpdateRoleAsync(roleName, role, cancellationToken);
        if (!result)
        {
            throw new InvalidOperationException($"Nie udało się zaktualizować roli {roleName}");
        }
    }

    /// <inheritdoc />
    public async Task DeleteRoleAsync(string roleName, CancellationToken cancellationToken = default)
    {
        var result = await _roleClient.DeleteRoleAsync(roleName, cancellationToken);
        if (!result)
        {
            throw new InvalidOperationException($"Nie udało się usunąć roli {roleName}");
        }
    }

    /// <inheritdoc />
    public async Task<List<KeycloakUser>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        return await _userClient.GetAllUsersAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<string?> GetUserProfilePictureUrlAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _userClient.GetUserProfilePictureUrlAsync(userId, cancellationToken);
    }
}
