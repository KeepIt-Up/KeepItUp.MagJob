
namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Odpowiedź zawierająca identyfikator utworzonej roli.
/// </summary>
public class CreateRoleResponse
{
  /// <summary>
  /// Identyfikator utworzonej roli.
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// Nazwa roli.
  /// </summary>
  public string Name { get; set; } = string.Empty;
}