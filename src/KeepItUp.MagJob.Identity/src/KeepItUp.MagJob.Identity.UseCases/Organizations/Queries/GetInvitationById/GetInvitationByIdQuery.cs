using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetInvitationById;

/// <summary>
/// Query to get an invitation by its identifier.
/// </summary>
public record GetInvitationByIdQuery : IRequest<Result<InvitationDto>>
{
    /// <summary>
    /// Organization identifier.
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// Invitation identifier.
    /// </summary>
    public Guid InvitationId { get; init; }

    /// <summary>
    /// User identifier performing the query.
    /// </summary>
    public Guid UserId { get; init; }
}
