using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetOrganizationById;

/// <summary>
/// Zapytanie o organizację na podstawie identyfikatora.
/// </summary>
public record GetOrganizationByIdQuery : IRequest<Result<OrganizationDto>>
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// Identyfikator użytkownika wykonującego zapytanie.
    /// </summary>
    public Guid UserId { get; init; }
}
