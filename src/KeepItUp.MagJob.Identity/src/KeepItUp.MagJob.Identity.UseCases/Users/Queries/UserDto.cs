namespace KeepItUp.MagJob.Identity.UseCases.Users.Queries;

/// <summary>
/// Data Transfer Object for the user.
/// </summary>
public class UserDto
{
    /// <summary>
    /// User identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// External user identifier in the external system (Keycloak).
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
    /// Whether the user is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// User profile.
    /// </summary>
    public UserProfileDto? Profile { get; set; }

    /// <summary>
    /// List of user memberships in organizations.
    /// </summary>
    public List<MembershipDto> Memberships { get; set; } = new();
}

/// <summary>
/// Data Transfer Object for the user profile.
/// </summary>
public class UserProfileDto
{
    /// <summary>
    /// User phone number.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// User address.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// URL of the user's profile picture.
    /// </summary>
    public string? ProfileImageUrl { get; set; }
}

/// <summary>
/// Data Transfer Object for the user's membership in an organization.
/// </summary>
public class MembershipDto
{
    /// <summary>
    /// Membership identifier.
    /// </summary>
    public Guid MemberId { get; set; }

    /// <summary>
    /// Organization identifier.
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// Date of joining the organization.
    /// </summary>
    public DateTime JoinedAt { get; set; }

    /// <summary>
    /// List of identifiers of roles assigned to the member.
    /// </summary>
    public List<string> Roles { get; set; } = new();
}
