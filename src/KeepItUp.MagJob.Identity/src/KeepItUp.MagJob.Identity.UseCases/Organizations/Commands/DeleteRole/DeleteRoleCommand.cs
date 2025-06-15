using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.DeleteRole;

/// <summary>
/// Command to delete a role from an organization.
/// </summary>
public record DeleteRoleCommand : IRequest<Result<EmptyResponse>>
{
    /// <summary>
    /// Organization identifier.
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// Role identifier.
    /// </summary>
    public Guid RoleId { get; init; }

    /// <summary>
    /// User identifier performing the operation.
    /// </summary>
    public Guid UserId { get; init; }
}
