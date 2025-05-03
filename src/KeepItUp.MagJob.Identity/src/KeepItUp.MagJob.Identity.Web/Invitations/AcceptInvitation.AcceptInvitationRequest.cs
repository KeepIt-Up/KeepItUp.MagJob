
namespace KeepItUp.MagJob.Identity.Web.Invitations;

/// <summary>
/// Żądanie akceptacji zaproszenia do organizacji.
/// </summary>
public class AcceptInvitationRequest
{
    public const string Route = "/Invitations/{InvitationId:guid}/accept";
    public static string BuildRoute(Guid invitationId) => Route.Replace("{InvitationId:guid}", invitationId.ToString());

    /// <summary>
    /// Identyfikator zaproszenia.
    /// </summary>
    public Guid InvitationId { get; set; }

    /// <summary>
    /// Token zaproszenia.
    /// </summary>
    public string Token { get; set; } = string.Empty;
}
