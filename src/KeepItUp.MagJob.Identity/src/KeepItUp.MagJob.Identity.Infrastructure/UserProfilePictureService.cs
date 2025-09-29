using KeepItUp.MagJob.Identity.Core.Interfaces;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;

namespace KeepItUp.MagJob.Identity.Infrastructure;

/// <summary>
/// Service for managing user profile pictures
/// </summary>
public class UserProfilePictureService : IUserProfilePictureService
{
    private readonly IKeycloakClient _keycloakClient;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserProfilePictureService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserProfilePictureService"/> class.
    /// </summary>
    /// <param name="keycloakClient">Keycloak client.</param>
    /// <param name="userRepository">User repository.</param>
    /// <param name="logger">Logger.</param>
    public UserProfilePictureService(
        IKeycloakClient keycloakClient,
        IUserRepository userRepository,
        ILogger<UserProfilePictureService> logger)
    {
        _keycloakClient = keycloakClient;
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<string?> GetProfilePictureUrlAsync(Guid userId, Guid externalId, bool forceRefresh = false, CancellationToken cancellationToken = default)
    {
        // Get the user from the repository
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return null;
        }

        // If the user has a profile picture and forceRefresh is not set, return it
        if (user.Profile?.ProfileImage != null && !forceRefresh)
        {
            return user.Profile.ProfileImage;
        }

        // Otherwise, get the profile picture from the IDP
        return await SyncProfilePictureFromIdpAsync(userId, externalId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<string?> SyncProfilePictureFromIdpAsync(Guid userId, Guid externalId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Get the profile picture URL from Keycloak/IDP
            var profilePictureUrl = await _keycloakClient.GetUserProfilePictureUrlAsync(externalId.ToString(), cancellationToken);

            if (string.IsNullOrEmpty(profilePictureUrl))
            {
                _logger.LogInformation("Użytkownik {UserId} nie ma zdjęcia profilowego w IDP", userId);
                return null;
            }

            // Get the user from the repository
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("Nie znaleziono użytkownika o ID {UserId}", userId);
                return null;
            }

            // Update the user's profile properties
            user.UpdateProfileProperties(profileImage: profilePictureUrl);

            // Save the changes to the repository
            await _userRepository.UpdateAsync(user, cancellationToken);

            _logger.LogInformation("Zaktualizowano zdjęcie profilowe użytkownika {UserId} z IDP", userId);

            return profilePictureUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas synchronizacji zdjęcia profilowego użytkownika {UserId} z IDP", userId);
            return null;
        }
    }
}
