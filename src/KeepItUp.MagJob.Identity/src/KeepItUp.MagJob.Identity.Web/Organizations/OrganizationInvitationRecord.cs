namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// DTO for an invitation to an organization.
/// </summary>
public class OrganizationInvitationRecord
{
    /// <summary>
    /// Invitation identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Email address of the invited person.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Invitation status.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Invitation expiration date.
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Whether the invitation has expired.
    /// </summary>
    public bool IsExpired { get; set; }

    /// <summary>
    /// Creation date of the invitation.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// User identifier who created the invitation.
    /// </summary>
    public Guid CreatedBy { get; set; }

    /// <summary>
    /// Role that will be assigned after accepting the invitation.
    /// </summary>
    public OrganizationRoleRecord? Role { get; set; }
}
