namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetRolesByOrganizationId;

/// <summary>
/// Zapytanie o role w organizacji.
/// </summary>
public class GetRolesByOrganizationIdQuery : PaginationQuery<RoleDto>
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
