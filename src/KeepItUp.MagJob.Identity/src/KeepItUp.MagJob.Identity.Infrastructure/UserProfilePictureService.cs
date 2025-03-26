using KeepItUp.MagJob.Identity.Core.Interfaces;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.Infrastructure;

/// <summary>
/// Serwis do zarządzania zdjęciami profilowymi użytkowników
/// </summary>
public class UserProfilePictureService : IUserProfilePictureService
{
    private readonly IKeycloakClient _keycloakClient;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserProfilePictureService> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="UserProfilePictureService"/>.
    /// </summary>
    /// <param name="keycloakClient">Klient Keycloak.</param>
    /// <param name="userRepository">Repozytorium użytkowników.</param>
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
        // Pobierz użytkownika z repozytorium
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return null;
        }

        // Jeśli użytkownik ma już zdjęcie profilowe i nie wymuszono odświeżenia, zwróć je
        if (user.Profile?.ProfileImage != null && !forceRefresh)
        {
            return user.Profile.ProfileImage;
        }

        // W przeciwnym razie pobierz zdjęcie z IDP
        return await SyncProfilePictureFromIdpAsync(userId, externalId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<string?> SyncProfilePictureFromIdpAsync(Guid userId, Guid externalId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Pobierz URL zdjęcia profilowego z Keycloak/IDP
            var profilePictureUrl = await _keycloakClient.GetUserProfilePictureUrlAsync(externalId.ToString(), cancellationToken);

            if (string.IsNullOrEmpty(profilePictureUrl))
            {
                _logger.LogInformation("Użytkownik {UserId} nie ma zdjęcia profilowego w IDP", userId);
                return null;
            }

            // Pobierz użytkownika z repozytorium
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("Nie znaleziono użytkownika o ID {UserId}", userId);
                return null;
            }

            // Zaktualizuj profil użytkownika
            user.UpdateProfileProperties(profileImage: profilePictureUrl);

            // Zapisz zmiany w repozytorium
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
