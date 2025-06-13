namespace KeepItUp.MagJob.Identity.Web.Users;

/// <summary>
/// Request for the UpdateUserProfilePictureEndpoint.
/// </summary>
public class UpdateUserProfilePictureRequest
{
    /// <summary>
    /// URL template for the UpdateUserProfilePictureEndpoint.
    /// </summary>
    public const string Route = "/Users/{UserId:guid}/profile-picture";

    /// <summary>
    /// Builds the URL for the specified user identifier.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <returns>URL with the specified identifier.</returns>
    public static string BuildRoute(Guid userId) => Route.Replace("{UserId:guid}", userId.ToString());

    /// <summary>
    /// User identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Profile picture file.
    /// </summary>
    public IFormFile? ProfilePictureFile { get; set; }
}
