using KeepItUp.MagJob.Identity.UseCases.Invitations.Queries;

namespace KeepItUp.MagJob.Identity.UseCases.Invitations.Queries.GetInvitations;

/// <summary>
/// Query to get invitations for an organization.
/// </summary>
public class GetInvitationsQuery : PaginationQuery<InvitationDto>
{
    /// <summary>
    /// Organization identifier.
    /// </summary>
    public Guid? OrganizationId { get; init; }

    /// <summary>
    /// Email address of the user.
    /// </summary>
    public string? Email { get; init; }
}