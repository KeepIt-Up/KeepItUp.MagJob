namespace KeepItUp.MagJob.Identity.Core.Interfaces;

/// <summary>
/// Interfejs serwisu do zarządzania zdjęciami profilowymi użytkowników
/// </summary>
public interface IUserProfilePictureService
{
    /// <summary>
    /// Pobiera URL zdjęcia profilowego użytkownika
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika w module Identity</param>
    /// <param name="externalId">Identyfikator użytkownika w Keycloak</param>
    /// <param name="forceRefresh">Czy wymusić odświeżenie zdjęcia z IDP</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>URL zdjęcia profilowego lub null, jeśli użytkownik nie ma zdjęcia</returns>
    Task<string?> GetProfilePictureUrlAsync(Guid userId, Guid externalId, bool forceRefresh = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Synchronizuje zdjęcie profilowe użytkownika z IDP
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika w module Identity</param>
    /// <param name="externalId">Identyfikator użytkownika w Keycloak</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>URL zaktualizowanego zdjęcia profilowego lub null, jeśli użytkownik nie ma zdjęcia w IDP</returns>
    Task<string?> SyncProfilePictureFromIdpAsync(Guid userId, Guid externalId, CancellationToken cancellationToken = default);
}
