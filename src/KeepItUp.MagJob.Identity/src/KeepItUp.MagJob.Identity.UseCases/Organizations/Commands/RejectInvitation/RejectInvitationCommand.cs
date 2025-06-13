using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.RejectInvitation;

/// <summary>
/// Command to reject an invitation to an organization.
/// </summary>
public record RejectInvitationCommand : IRequest<Result>
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
    /// User identifier rejecting the invitation.
    /// </summary>
    public Guid UserId { get; init; }
}
