using System.Text.Json.Serialization;

namespace KeepItUp.MagJob.Identity.Core.Keycloak;

/// <summary>
/// Represents an organization in the context of Keycloak
/// </summary>
public class KeycloakOrganization
{
    /// <summary>
    /// Organization ID
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    /// <summary>
    /// Organization name
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>
    /// List of user roles in the organization
    /// </summary>
    [JsonPropertyName("roles")]
    public required List<string> Roles { get; set; }
}
