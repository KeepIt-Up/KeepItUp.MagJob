
namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Żądanie utworzenia nowej roli w organizacji.
/// </summary>
public class CreateRoleRequest
{
  public const string Route = "/Organizations/{OrganizationId:guid}/Roles";
  public static string BuildRoute(Guid organizationId) => Route.Replace("{OrganizationId:guid}", organizationId.ToString());

  /// <summary>
  /// Identyfikator organizacji.
  /// </summary>
  public Guid OrganizationId { get; set; }

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