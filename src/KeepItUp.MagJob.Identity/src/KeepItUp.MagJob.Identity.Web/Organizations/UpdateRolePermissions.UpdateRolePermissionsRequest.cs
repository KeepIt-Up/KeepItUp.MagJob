
namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Request for the UpdateRolePermissionsEndpoint.
/// </summary>
public class UpdateRolePermissionsRequest
{
    public const string Route = "/Organizations/{OrganizationId:guid}/Roles/{RoleId:guid}/Permissions";
    public static string BuildRoute(Guid organizationId, Guid roleId) => Route.Replace("{OrganizationId:guid}", organizationId.ToString()).Replace("{RoleId:guid}", roleId.ToString());

    /// <summary>
    /// Organization identifier.
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// Role identifier.
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// List of permission names to assign to the role.
    /// </summary>
    public List<string> Permissions { get; set; } = new();
}