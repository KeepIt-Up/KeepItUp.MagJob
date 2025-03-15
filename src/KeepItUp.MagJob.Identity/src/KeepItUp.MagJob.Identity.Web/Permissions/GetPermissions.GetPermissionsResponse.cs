
using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetPermissions;

namespace KeepItUp.MagJob.Identity.Web.Permissions;

/// <summary>
/// Odpowiedź zawierająca listę uprawnień.
/// </summary>
public class GetPermissionsResponse
{
  /// <summary>
  /// Lista uprawnień.
  /// </summary>
  public List<PermissionDto> Permissions { get; set; } = new();
}