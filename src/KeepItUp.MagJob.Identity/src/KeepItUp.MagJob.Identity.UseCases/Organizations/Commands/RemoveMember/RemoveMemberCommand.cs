using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.RemoveMember;

/// <summary>
/// Command to remove a member from an organization.
/// </summary>
public record RemoveMemberCommand : IRequest<Result>
{
    /// <summary>
    /// Organization identifier.
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// User identifier to remove.
    /// </summary>
    public Guid MemberUserId { get; init; }

    /// <summary>
    /// User identifier performing the operation.
    /// </summary>
    public Guid RequestingUserId { get; init; }
}
