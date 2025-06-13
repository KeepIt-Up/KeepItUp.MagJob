using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetRoleById;

/// <summary>
/// Query to get a role by its identifier.
/// </summary>
public record GetRoleByIdQuery : IRequest<Result<RoleDto>>
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
    /// User identifier performing the query.
    /// </summary>
    public Guid UserId { get; init; }
}
