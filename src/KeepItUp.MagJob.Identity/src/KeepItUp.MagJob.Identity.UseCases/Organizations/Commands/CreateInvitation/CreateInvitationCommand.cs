using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.CreateInvitation;

/// <summary>
/// Command to create an invitation to an organization.
/// </summary>
public record CreateInvitationCommand : IRequest<Result<Guid>>
{
    /// <summary>
    /// Organization identifier.
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// Email address of the person being invited.
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Role identifier to be assigned after accepting the invitation.
    /// </summary>
    public Guid RoleId { get; init; }

    /// <summary>
    /// User identifier performing the operation.
    /// </summary>
    public Guid UserId { get; init; }
}
