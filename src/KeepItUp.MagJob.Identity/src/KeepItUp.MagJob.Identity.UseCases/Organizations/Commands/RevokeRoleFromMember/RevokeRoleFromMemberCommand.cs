using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.RevokeRoleFromMember;

/// <summary>
/// Command to revoke a role from a member of an organization.
/// </summary>
public record RevokeRoleFromMemberCommand : IRequest<Result<EmptyResponse>>
{
    /// <summary>
    /// Organization identifier.
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// User identifier to revoke the role from.
    /// </summary>
    public Guid MemberUserId { get; init; }

    /// <summary>
    /// Role identifier to revoke.
    /// </summary>
    public Guid RoleId { get; init; }

    /// <summary>
    /// User identifier performing the operation.
    /// </summary>
    public Guid RequestingUserId { get; init; }
}
