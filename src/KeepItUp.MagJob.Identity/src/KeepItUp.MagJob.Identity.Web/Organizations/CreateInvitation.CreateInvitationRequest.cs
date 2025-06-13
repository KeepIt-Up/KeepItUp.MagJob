
namespace KeepItUp.MagJob.Identity.Web.Organizations;
/// <summary>
/// Request to create an invitation to an organization.
/// </summary>
public class CreateInvitationRequest
{
    public const string Route = "/Organizations/{OrganizationId:guid}/Invitations";
    public static string BuildRoute(Guid organizationId) => Route.Replace("{OrganizationId:guid}", organizationId.ToString());

    /// <summary>
    /// Organization identifier.
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// Email address of the person being invited.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Role identifier to assign after accepting the invitation.
    /// </summary>
    public Guid RoleId { get; set; }
}