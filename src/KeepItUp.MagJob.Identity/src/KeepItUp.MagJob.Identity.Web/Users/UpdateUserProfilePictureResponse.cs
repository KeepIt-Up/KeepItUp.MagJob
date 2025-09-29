namespace KeepItUp.MagJob.Identity.Web.Users;

/// <summary>
/// Response for the UpdateUserProfilePictureEndpoint.
/// </summary>
public class UpdateUserProfilePictureResponse
{
    /// <summary>
    /// URL of the updated profile picture.
    /// </summary>
    public string? ProfileImageUrl { get; set; }
}
