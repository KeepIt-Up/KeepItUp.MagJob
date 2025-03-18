
namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Odpowiedź dla endpointu GetInvitationsEndpoint.
/// </summary>
public class GetInvitationsResponse
{
    /// <summary>
    /// Lista zaproszeń do organizacji.
    /// </summary>
    public List<OrganizationInvitationRecord> Invitations { get; set; } = new List<OrganizationInvitationRecord>();
}
