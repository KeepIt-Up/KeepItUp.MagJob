namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetOrganizationMembers;

/// <summary>
/// Query to get members of an organization.
/// </summary>
public class GetOrganizationMembersQuery : PaginationQuery<MemberDto>
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
