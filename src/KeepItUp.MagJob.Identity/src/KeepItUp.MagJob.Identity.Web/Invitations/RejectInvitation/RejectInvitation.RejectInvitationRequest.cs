
namespace KeepItUp.MagJob.Identity.Web.Invitations;

/// <summary>
/// Request to reject an invitation to an organization.
/// </summary>
public class RejectInvitationRequest
{
    public const string Route = "/Invitations/{InvitationId:guid}";

    /// <summary>
    /// Invitation identifier.
    /// </summary>
    public Guid InvitationId { get; set; }

    /// <summary>
    /// Invitation token.
    /// </summary>
    public string Token { get; set; } = string.Empty;
}
