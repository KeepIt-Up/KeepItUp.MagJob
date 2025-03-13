using System.Text.Json.Serialization;

namespace KeepItUp.MagJob.Identity.Core.Keycloak;

/// <summary>
/// Reprezentuje użytkownika w kontekście Keycloak
/// </summary>
public class KeycloakUser
{
    /// <summary>
    /// Identyfikator użytkownika w Keycloak
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    
    /// <summary>
    /// Nazwa użytkownika
    /// </summary>
    [JsonPropertyName("username")]
    public required string Username { get; set; }
    
    /// <summary>
    /// Adres email użytkownika
    /// </summary>
    [JsonPropertyName("email")]
    public required string Email { get; set; }
    
    /// <summary>
    /// Imię użytkownika
    /// </summary>
    [JsonPropertyName("firstName")]
    public string? FirstName { get; set; }
    
    /// <summary>
    /// Nazwisko użytkownika
    /// </summary>
    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }
    
    /// <summary>
    /// Określa, czy użytkownik jest aktywny
    /// </summary>
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }
    
    /// <summary>
    /// Określa, czy email użytkownika został zweryfikowany
    /// </summary>
    [JsonPropertyName("emailVerified")]
    public bool EmailVerified { get; set; }
    
    /// <summary>
    /// Atrybuty użytkownika
    /// </summary>
    [JsonPropertyName("attributes")]
    public Dictionary<string, List<string>>? Attributes { get; set; }
    
    /// <summary>
    /// Data utworzenia konta użytkownika
    /// </summary>
    [JsonPropertyName("createdTimestamp")]
    public long CreatedTimestamp { get; set; }
    
    /// <summary>
    /// Konwertuje timestamp na DateTime
    /// </summary>
    [JsonIgnore]
    public DateTime CreatedAt => DateTimeOffset.FromUnixTimeMilliseconds(CreatedTimestamp).DateTime;
} 
