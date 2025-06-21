namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Żądanie usunięcia roli z organizacji.
/// </summary>
public class DeleteRoleRequest
{
    /// <summary>
    /// Szablon ścieżki URL dla endpointu usuwania roli.
    /// </summary>
    public const string Route = "/Organizations/{OrganizationId:guid}/Roles/{RoleId:guid}";

    /// <summary>
    /// Buduje ścieżkę URL dla określonego identyfikatora organizacji i identyfikatora roli.
    /// </summary>
    /// <param name="organizationId">Identyfikator organizacji.</param>
    /// <param name="roleId">Identyfikator roli do usunięcia.</param>
    /// <returns>Ścieżka URL z uwzględnionymi identyfikatorami.</returns>
    public static string BuildRoute(Guid organizationId, Guid roleId) => Route.Replace("{OrganizationId:guid}", organizationId.ToString()).Replace("{RoleId:guid}", roleId.ToString());

    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// Identyfikator roli.
    /// </summary>
    public Guid RoleId { get; set; }
}
