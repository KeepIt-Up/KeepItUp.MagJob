using KeepItUp.MagJob.Identity.UseCases.Invitations.Queries;

namespace KeepItUp.MagJob.Identity.Web.Invitations;

/// <summary>
/// Request for the GetInvitationsEndpoint.
/// </summary>
public class GetInvitationsRequest : PaginationRequest<InvitationDto>
{
    public const string Route = "/Invitations";

    /// <summary>
    /// Organization identifier.
    /// </summary>
    public Guid? OrganizationId { get; set; }

    /// <summary>
    /// Email address of the user.
    /// </summary>
    public string? Email { get; set; }
}
