
namespace KeepItUp.MagJob.Identity.Web.Invitations;
/// <summary>
/// Request to create an invitation to an organization.
/// </summary>
public class CreateInvitationRequest
{
    public const string Route = "/Invitations";

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