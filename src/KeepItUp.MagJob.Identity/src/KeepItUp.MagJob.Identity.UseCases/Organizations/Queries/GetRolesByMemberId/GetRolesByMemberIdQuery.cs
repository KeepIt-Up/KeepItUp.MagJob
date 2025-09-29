namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetRolesByMemberId;

/// <summary>
/// Query to get roles assigned to a member of an organization.
/// </summary>
public class GetRolesByMemberIdQuery : PaginationQuery<RoleDto>
{
    /// <summary>
    /// Organization identifier.
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// User identifier of the member whose roles we want to get.
    /// </summary>
    public Guid MemberUserId { get; init; }

    /// <summary>
    /// User identifier performing the query.
    /// </summary>
    public Guid RequestingUserId { get; init; }
}
