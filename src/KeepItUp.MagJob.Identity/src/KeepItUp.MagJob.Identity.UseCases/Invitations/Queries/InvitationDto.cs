namespace KeepItUp.MagJob.Identity.UseCases.Invitations.Queries;

/// <summary>
/// DTO for an invitation to an organization.
/// </summary>
public class InvitationDto
{
    /// <summary>
    /// Invitation identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Organization identifier.
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// Email address of the invited user.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Invitation token.
    /// </summary>
    public string Token { get; set; } = string.Empty;

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
}
