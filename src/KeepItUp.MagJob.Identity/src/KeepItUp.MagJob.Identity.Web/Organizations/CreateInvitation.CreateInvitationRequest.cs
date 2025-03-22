
namespace KeepItUp.MagJob.Identity.Web.Organizations;
/// <summary>
/// Żądanie utworzenia zaproszenia do organizacji.
/// </summary>
public class CreateInvitationRequest
{ 
  public const string Route = "/Organizations/{OrganizationId:guid}/Invitations";
  public static string BuildRoute(Guid organizationId) => Route.Replace("{OrganizationId:guid}", organizationId.ToString());

  /// <summary>
  /// Identyfikator organizacji.
  /// </summary>
  public Guid OrganizationId { get; set; }

  /// <summary>
  /// Adres email osoby zapraszanej.
  /// </summary>
  public string Email { get; set; } = string.Empty;

  /// <summary>
  /// Identyfikator roli, która zostanie przypisana po akceptacji zaproszenia.
  /// </summary>
  public Guid RoleId { get; set; }
}