
namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Odpowiedź dla endpointu CreateOrganizationEndpoint.
/// </summary>
public class CreateOrganizationResponse
{
  /// <summary>
  /// Identyfikator organizacji.
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// Nazwa organizacji.
  /// </summary>
  public string Name { get; set; } = string.Empty;

  /// <summary>
  /// Opis organizacji.
  /// </summary>
  public string? Description { get; set; }

  /// <summary>
  /// Identyfikator właściciela organizacji.
  /// </summary>
  public Guid OwnerId { get; set; }
}