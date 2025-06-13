using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.CreateRole;

/// <summary>
/// Command to create a new role in an organization.
/// </summary>
public record CreateRoleCommand : IRequest<Result<Guid>>
{
    /// <summary>
    /// Organization identifier.
    /// </summary>
    public Guid OrganizationId { get; init; }

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
