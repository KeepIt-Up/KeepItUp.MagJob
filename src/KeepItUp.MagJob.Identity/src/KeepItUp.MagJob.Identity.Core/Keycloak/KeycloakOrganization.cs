using System.Text.Json.Serialization;

namespace KeepItUp.MagJob.Identity.Core.Keycloak;

/// <summary>
/// Reprezentuje organizację w kontekście Keycloak
/// </summary>
public class KeycloakOrganization
{
    /// <summary>
    /// Identyfikator organizacji
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    /// <summary>
    /// Nazwa organizacji
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>
    /// Lista ról użytkownika w organizacji
    /// </summary>
    [JsonPropertyName("roles")]
    public required List<string> Roles { get; set; }
}
