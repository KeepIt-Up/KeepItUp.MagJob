using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateRole;

/// <summary>
/// Command to update an existing role in an organization.
/// </summary>
public record UpdateRoleCommand : IRequest<Result<EmptyResponse>>
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
    /// Role name.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Role description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Role color (in HEX format).
    /// </summary>
    public string? Color { get; init; }

    /// <summary>
    /// User identifier performing the operation.
    /// </summary>
    public Guid UserId { get; init; }
}
