using System.Text.Json.Serialization;

namespace KeepItUp.MagJob.Identity.Core.Keycloak;

/// <summary>
/// Reprezentuje zdarzenie z Keycloak
/// </summary>
public class KeycloakEvent
{
    /// <summary>
    /// Pobiera lub ustawia identyfikator zdarzenia
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Pobiera lub ustawia czas zdarzenia (w milisekundach od epoki Unix)
    /// </summary>
    [JsonPropertyName("time")]
    public long Time { get; set; }

    /// <summary>
    /// Pobiera lub ustawia typ zdarzenia
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Pobiera lub ustawia identyfikator realm
    /// </summary>
    [JsonPropertyName("realmId")]
    public string RealmId { get; set; } = string.Empty;

    /// <summary>
    /// Pobiera lub ustawia identyfikator klienta
    /// </summary>
    [JsonPropertyName("clientId")]
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Pobiera lub ustawia identyfikator użytkownika
    /// </summary>
    [JsonPropertyName("userId")]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Pobiera lub ustawia nazwę sesji
    /// </summary>
    [JsonPropertyName("sessionId")]
    public string SessionId { get; set; } = string.Empty;

    /// <summary>
    /// Pobiera lub ustawia adres IP
    /// </summary>
    [JsonPropertyName("ipAddress")]
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// Pobiera lub ustawia błąd
    /// </summary>
    [JsonPropertyName("error")]
    public string Error { get; set; } = string.Empty;

    /// <summary>
    /// Pobiera lub ustawia szczegóły zdarzenia
    /// </summary>
    [JsonPropertyName("details")]
    public Dictionary<string, string> Details { get; set; } = new Dictionary<string, string>();
}
