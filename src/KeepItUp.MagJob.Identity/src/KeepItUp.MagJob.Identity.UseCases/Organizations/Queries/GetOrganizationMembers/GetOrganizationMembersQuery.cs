namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetOrganizationMembers;

/// <summary>
/// Zapytanie o członków organizacji.
/// </summary>
public class GetOrganizationMembersQuery : PaginationQuery<MemberDto>
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
