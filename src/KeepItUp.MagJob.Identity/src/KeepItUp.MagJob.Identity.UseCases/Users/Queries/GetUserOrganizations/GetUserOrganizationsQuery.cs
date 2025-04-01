using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;

namespace KeepItUp.MagJob.Identity.UseCases.Users.Queries.GetUserOrganizations;

/// <summary>
/// Zapytanie o organizacje, do których należy użytkownik.
/// </summary>
public class GetUserOrganizationsQuery : PaginationQuery<OrganizationDto>
{
    /// <summary>
    /// Identyfikator użytkownika.
    /// </summary>
    public Guid UserId { get; init; }
}
