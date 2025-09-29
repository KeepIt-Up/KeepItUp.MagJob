
namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Request to create a new role in an organization.
/// </summary>
public class CreateRoleRequest
{
    public const string Route = "/Organizations/{OrganizationId:guid}/Roles";
    public static string BuildRoute(Guid organizationId) => Route.Replace("{OrganizationId:guid}", organizationId.ToString());

    /// <summary>
    /// Organization identifier.
    /// </summary>
    public Guid OrganizationId { get; set; }

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