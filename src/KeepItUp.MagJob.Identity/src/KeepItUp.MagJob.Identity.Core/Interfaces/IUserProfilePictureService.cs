namespace KeepItUp.MagJob.Identity.Core.Interfaces;

/// <summary>
/// Interface for managing user profile pictures
/// </summary>
public interface IUserProfilePictureService
{
    /// <summary>
    /// Gets the URL of the user's profile picture
    /// </summary>
    /// <param name="userId">User ID in the Identity module</param>
    /// <param name="externalId">User ID in Keycloak</param>
    /// <param name="forceRefresh">Whether to force a refresh of the picture from IDP</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>URL of the user's profile picture or null if the user has no picture</returns>
    Task<string?> GetProfilePictureUrlAsync(Guid userId, Guid externalId, bool forceRefresh = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Synchronizes the user's profile picture with IDP
    /// </summary>
    /// <param name="userId">User ID in the Identity module</param>
    /// <param name="externalId">User ID in Keycloak</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>URL of the updated profile picture or null if the user has no picture in IDP</returns>
    Task<string?> SyncProfilePictureFromIdpAsync(Guid userId, Guid externalId, CancellationToken cancellationToken = default);
}
