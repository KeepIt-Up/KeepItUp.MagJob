using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetOrganizationById;

/// <summary>
/// Query to get an organization by its identifier.
/// </summary>
public record GetOrganizationByIdQuery : IRequest<Result<OrganizationDto>>
{
    /// <summary>
    /// Organization identifier.
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// User identifier performing the query.
    /// </summary>
    public Guid UserId { get; init; }
}
