namespace KeepItUp.MagJob.Identity.Web.Users;

/// <summary>
/// DTO for the organization in the context of the user.
/// </summary>
public class UserOrganizationRecord
{
    /// <summary>
    /// Organization identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Organization name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Organization description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Owner organization identifier.
    /// </summary>
    public Guid OwnerId { get; set; }

    /// <summary>
    /// Determines if the user is the owner of the organization.
    /// </summary>
    public bool IsOwner { get; set; }

    /// <summary>
    /// Number of organization members.
    /// </summary>
    public int MemberCount { get; set; }
}
