using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;
using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Users.Queries.GetUserOrganizations;

/// <summary>
/// Zapytanie o organizacje, do których należy użytkownik.
/// </summary>
public record GetUserOrganizationsQuery : IRequest<Result<List<OrganizationDto>>>
{
    /// <summary>
    /// Identyfikator użytkownika.
    /// </summary>
    public Guid UserId { get; init; }
}
