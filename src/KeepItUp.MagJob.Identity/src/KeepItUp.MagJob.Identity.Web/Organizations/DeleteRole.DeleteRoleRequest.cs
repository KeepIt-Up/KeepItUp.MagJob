
namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Żądanie usunięcia roli z organizacji.
/// </summary>
public class DeleteRoleRequest
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
}