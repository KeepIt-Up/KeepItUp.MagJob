
namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Żądanie aktualizacji uprawnień roli w organizacji.
/// </summary>
public class UpdateRolePermissionsRequest
{
    public const string Route = "/Organizations/{OrganizationId:guid}/Roles/{RoleId:guid}/Permissions";
    public static string BuildRoute(Guid organizationId, Guid roleId) => Route.Replace("{OrganizationId:guid}", organizationId.ToString()).Replace("{RoleId:guid}", roleId.ToString());

    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// Identyfikator roli.
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// Lista nazw uprawnień do przypisania do roli.
    /// </summary>
    public List<string> Permissions { get; set; } = new();
}