using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Invitations.Commands.RejectInvitation;

/// <summary>
/// Command to reject an invitation.
/// </summary>
public record RejectInvitationCommand : IRequest<Result>
{
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