using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;

namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Żądanie pobrania członków organizacji.
/// </summary>
public class GetOrganizationMembersRequest : PaginationRequest<MemberDto>
{
    public const string Route = "/Organizations/{OrganizationId:guid}/Members";
    public static string BuildRoute(Guid organizationId) => Route.Replace("{OrganizationId:guid}", organizationId.ToString());

    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; set; }
}
