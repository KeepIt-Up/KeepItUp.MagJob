
namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Żądanie pobrania ról organizacji.
/// </summary>
public class GetOrganizationRolesRequest
{
    public const string Route = "/Organizations/{OrganizationId:guid}/Roles";
    public static string BuildRoute(Guid organizationId) => Route.Replace("{OrganizationId:guid}", organizationId.ToString());

    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; set; }
}