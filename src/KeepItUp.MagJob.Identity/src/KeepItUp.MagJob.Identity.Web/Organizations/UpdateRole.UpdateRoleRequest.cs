
namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Żądanie aktualizacji roli w organizacji.
/// </summary>
public class UpdateRoleRequest
{
    public const string Route = "/Organizations/{OrganizationId:guid}/Roles/{RoleId:guid}";
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
    /// Nazwa roli.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Opis roli.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Kolor roli (w formacie HEX).
    /// </summary>
    public string? Color { get; set; }
}

