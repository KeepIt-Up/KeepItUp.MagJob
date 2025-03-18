
namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Żądanie odrzucenia zaproszenia do organizacji.
/// </summary>
public class RejectInvitationRequest
{
    public const string Route = "/Organizations/{OrganizationId:guid}/Invitations/{InvitationId:guid}";
    public static string BuildRoute(Guid organizationId, Guid invitationId) => Route.Replace("{OrganizationId:guid}", organizationId.ToString()).Replace("{InvitationId:guid}", invitationId.ToString());

    /// <summary>
    /// Identyfikator zaproszenia.
    /// </summary>
    public Guid InvitationId { get; set; }

    /// <summary>
    /// Token zaproszenia.
    /// </summary>
    public string Token { get; set; } = string.Empty;
}
