using KeepItUp.MagJob.Identity.UseCases.Users.Queries;

namespace KeepItUp.MagJob.Identity.Web.Users;

/// <summary>
/// Response for the GetUserEndpoint.
/// </summary>
public class GetUserByIdResponse
{
    /// <summary>
    /// User identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// External user identifier (e.g. from Keycloak).
    /// </summary>
    public Guid ExternalId { get; set; }

    /// <summary>
    /// User email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User first name.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User last name.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Determines if the user is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// URL of the user's profile picture.
    /// </summary>
    public string? ProfileImageUrl { get; set; }

    /// <summary>
    /// List of organizations the user belongs to.
    /// </summary>
    public List<MembershipDto> Memberships { get; set; } = new();
}
