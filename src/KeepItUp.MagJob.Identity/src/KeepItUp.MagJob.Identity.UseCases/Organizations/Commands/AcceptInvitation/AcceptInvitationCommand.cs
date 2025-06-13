using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.AcceptInvitation;

/// <summary>
/// Command to accept an invitation to an organization.
/// </summary>
public record AcceptInvitationCommand : IRequest<Result<Guid>>
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
    /// Invitation token.
    /// </summary>
    public string Token { get; init; } = string.Empty;

    /// <summary>
    /// User identifier accepting the invitation.
    /// </summary>
    public Guid UserId { get; init; }
}
