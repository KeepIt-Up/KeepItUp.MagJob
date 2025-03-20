
using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;
using KeepItUp.MagJob.SharedKernel.Pagination;

namespace KeepItUp.MagJob.Identity.UseCases.Users.Queries.GetUserOrganizationsPaged;

/// <summary>
/// Zapytanie o stronicowaną listę organizacji, do których należy użytkownik.
/// </summary>
public class GetUserOrganizationsPagedQuery : PaginationQuery<OrganizationDto>
{
    /// <summary>
    /// Identyfikator użytkownika.
    /// </summary>
    public Guid UserId { get; init; }
}
