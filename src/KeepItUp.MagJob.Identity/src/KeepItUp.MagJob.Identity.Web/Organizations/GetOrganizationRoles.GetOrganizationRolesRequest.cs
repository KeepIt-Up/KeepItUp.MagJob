
namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Request to get the roles of an organization.
/// </summary>
public class GetOrganizationRolesRequest
{
    public const string Route = "/Organizations/{OrganizationId:guid}/Roles";
    public static string BuildRoute(Guid organizationId) => Route.Replace("{OrganizationId:guid}", organizationId.ToString());

    /// <summary>
    /// Organization identifier.
    /// </summary>
    public Guid OrganizationId { get; set; }
}