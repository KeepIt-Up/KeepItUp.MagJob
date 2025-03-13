
namespace KeepItUp.MagJob.Identity.Infrastructure.Keycloak;

/// <summary>
/// Opcje konfiguracji dla integracji z Keycloak
/// </summary>
public class KeycloakOptions
{
    /// <summary>
    /// Adres URL serwera Keycloak
    /// </summary>
    public required string ServerUrl { get; set; }
    
    /// <summary>
    /// Nazwa realmu w Keycloak
    /// </summary>
    public required string Realm { get; set; }
    
    /// <summary>
    /// Identyfikator klienta (client_id) używany do komunikacji z Keycloak
    /// </summary>
    public required string ClientId { get; set; }
    
    /// <summary>
    /// Sekret klienta (client_secret) używany do uwierzytelniania
    /// </summary>
    public required string ClientSecret { get; set; } = string.Empty;
    
    /// <summary>
    /// Adres URL do pobrania metadanych OpenID Connect
    /// </summary>
    public string MetadataUrl => $"{ServerUrl}/realms/{Realm}/.well-known/openid-configuration";
    
    /// <summary>
    /// Adres URL do uwierzytelniania
    /// </summary>
    public string AuthorityUrl => $"{ServerUrl}/realms/{Realm}";
    
    /// <summary>
    /// Adres URL do administracyjnego API Keycloak
    /// </summary>
    public string AdminUrl => $"{ServerUrl}/admin/realms/{Realm}";
    
    /// <summary>
    /// Określa, czy połączenie z Keycloak wymaga HTTPS
    /// </summary>
    public bool RequireHttps { get; set; } = true;
    
    /// <summary>
    /// Czas ważności tokenu w sekundach
    /// </summary>
    public int TokenExpirationSeconds { get; set; } = 300;
    
    /// <summary>
    /// Maksymalny czas oczekiwania na odpowiedź z Keycloak w sekundach
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;
    
    /// <summary>
    /// Maksymalny czas oczekiwania na odpowiedź z Keycloak w sekundach (alias dla TimeoutSeconds)
    /// </summary>
    public int MaxTimeoutSeconds => TimeoutSeconds;

    /// <summary>
    /// Nazwa użytkownika administratora Keycloak
    /// </summary>
    public string AdminUsername { get; set; } = string.Empty;
    
    /// <summary>
    /// Hasło administratora Keycloak
    /// </summary>
    public string AdminPassword { get; set; } = string.Empty;
    
    /// <summary>
    /// Identyfikator klienta używany do uwierzytelniania administratora (domyślnie "admin-cli")
    /// </summary>
    public string AdminClientId { get; set; } = "admin-cli";
} 
