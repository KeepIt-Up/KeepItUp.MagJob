using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;

namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Request to get the members of an organization.
/// </summary>
public class GetOrganizationMembersRequest : PaginationRequest<MemberDto>
{
    public const string Route = "/Organizations/{OrganizationId:guid}/Members";
    public static string BuildRoute(Guid organizationId) => Route.Replace("{OrganizationId:guid}", organizationId.ToString());

    /// <summary>
    /// Organization identifier.
    /// </summary>
    public Guid OrganizationId { get; set; }
}
