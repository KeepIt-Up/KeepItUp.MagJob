using System.Text.Json.Serialization;

namespace KeepItUp.MagJob.Identity.Core.Keycloak;

/// <summary>
/// Model reprezentujący rolę w Keycloak
/// </summary>
public class KeycloakRole
{
    /// <summary>
    /// Identyfikator roli
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Nazwa roli
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>
    /// Opis roli
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Określa, czy rola jest rolą klienta
    /// </summary>
    [JsonPropertyName("clientRole")]
    public bool ClientRole { get; set; }

    /// <summary>
    /// Określa, czy rola jest rolą kompozytową (składającą się z innych ról)
    /// </summary>
    [JsonPropertyName("composite")]
    public bool Composite { get; set; }

    /// <summary>
    /// Identyfikator kontenera (realm lub klienta)
    /// </summary>
    [JsonPropertyName("containerId")]
    public string? ContainerId { get; set; }

    /// <summary>
    /// Atrybuty roli
    /// </summary>
    [JsonPropertyName("attributes")]
    public Dictionary<string, List<string>>? Attributes { get; set; }
}
