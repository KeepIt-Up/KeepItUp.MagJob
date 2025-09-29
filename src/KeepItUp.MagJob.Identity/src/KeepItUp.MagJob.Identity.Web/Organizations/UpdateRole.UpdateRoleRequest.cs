
namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Request for the UpdateRoleEndpoint.
/// </summary>
public class UpdateRoleRequest
{
    public const string Route = "/Organizations/{OrganizationId:guid}/Roles/{RoleId:guid}";
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
    /// Role name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Role description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Role color (in HEX format).
    /// </summary>
    public string? Color { get; set; }
}

