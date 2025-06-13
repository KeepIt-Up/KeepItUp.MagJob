
namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Request to reject an invitation to an organization.
/// </summary>
public class RejectInvitationRequest
{
    public const string Route = "/Organizations/{OrganizationId:guid}/Invitations/{InvitationId:guid}";
    public static string BuildRoute(Guid organizationId, Guid invitationId) => Route.Replace("{OrganizationId:guid}", organizationId.ToString()).Replace("{InvitationId:guid}", invitationId.ToString());

    /// <summary>
    /// Invitation identifier.
    /// </summary>
    public Guid InvitationId { get; set; }

    /// <summary>
    /// Invitation token.
    /// </summary>
    public string Token { get; set; } = string.Empty;
}
