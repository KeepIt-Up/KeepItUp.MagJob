using System.Text.Json.Serialization;

namespace KeepItUp.MagJob.Identity.Core.Keycloak;

/// <summary>
/// Represents a role in Keycloak
/// </summary>
public class KeycloakRole
{
    /// <summary>
    /// Role ID
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Role name
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>
    /// Role description
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Determines if the role is a client role
    /// </summary>
    [JsonPropertyName("clientRole")]
    public bool ClientRole { get; set; }

    /// <summary>
    /// Determines if the role is a composite role (composed of other roles)
    /// </summary>
    [JsonPropertyName("composite")]
    public bool Composite { get; set; }

    /// <summary>
    /// Container ID (realm or client)
    /// </summary>
    [JsonPropertyName("containerId")]
    public string? ContainerId { get; set; }

    /// <summary>
    /// Role attributes
    /// </summary>
    [JsonPropertyName("attributes")]
    public Dictionary<string, List<string>>? Attributes { get; set; }
}
