
namespace KeepItUp.MagJob.Identity.Web.Invitations;

/// <summary>
/// Request to accept an invitation to an organization.
/// </summary>
public class AcceptInvitationRequest
{
    public const string Route = "/Invitations/{InvitationId:guid}/accept";
    public static string BuildRoute(Guid invitationId) => Route.Replace("{InvitationId:guid}", invitationId.ToString());

    /// <summary>
    /// Invitation identifier.
    /// </summary>
    public Guid InvitationId { get; set; }

    /// <summary>
    /// Invitation token.
    /// </summary>
    public string Token { get; set; } = string.Empty;
}
