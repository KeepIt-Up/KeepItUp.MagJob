using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.AssignRoleToMember;

/// <summary>
/// Command to assign a role to a member of an organization.
/// </summary>
public record AssignRoleToMemberCommand : IRequest<Result>
{
    /// <summary>
    /// Organization identifier.
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// User identifier to assign the role to.
    /// </summary>
    public Guid MemberUserId { get; init; }

    /// <summary>
    /// Role identifier to assign.
    /// </summary>
    public Guid RoleId { get; init; }

    /// <summary>
    /// User identifier performing the operation.
    /// </summary>
    public Guid RequestingUserId { get; init; }
}
