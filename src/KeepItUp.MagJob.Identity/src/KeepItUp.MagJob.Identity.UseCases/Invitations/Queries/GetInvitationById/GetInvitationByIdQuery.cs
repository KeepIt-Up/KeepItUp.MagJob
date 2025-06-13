using MediatR;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;

namespace KeepItUp.MagJob.Identity.UseCases.Invitations.Queries.GetInvitationById;

/// <summary>
/// Query to get an invitation by its identifier.
/// </summary>
public record GetInvitationByIdQuery : IRequest<Result<InvitationDto>>
{
    /// <summary>
    /// Invitation identifier.
    /// </summary>
    public Guid InvitationId { get; init; }

    /// <summary>
    /// User identifier performing the query.
    /// </summary>
    public Guid UserId { get; init; }
}