using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateRolePermissions;

/// <summary>
/// Command to update the permissions of a role in an organization.
/// </summary>
public record UpdateRolePermissionsCommand : IRequest<Result>
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
    /// List of permission names to assign to the role.
    /// </summary>
    public List<string> Permissions { get; init; } = new();

    /// <summary>
    /// User identifier performing the operation.
    /// </summary>
    public Guid UserId { get; init; }
}
