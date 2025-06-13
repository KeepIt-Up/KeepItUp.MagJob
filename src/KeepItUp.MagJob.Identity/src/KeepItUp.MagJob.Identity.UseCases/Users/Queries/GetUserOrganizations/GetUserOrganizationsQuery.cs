using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;

namespace KeepItUp.MagJob.Identity.UseCases.Users.Queries.GetUserOrganizations;

/// <summary>
/// Query to get organizations to which the user belongs.
/// </summary>
public class GetUserOrganizationsQuery : PaginationQuery<OrganizationDto>
{
    /// <summary>
    /// User identifier.
    /// </summary>
    public Guid UserId { get; init; }
}
