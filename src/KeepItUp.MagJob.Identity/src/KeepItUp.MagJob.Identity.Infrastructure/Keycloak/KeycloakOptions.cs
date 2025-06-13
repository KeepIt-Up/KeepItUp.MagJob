
namespace KeepItUp.MagJob.Identity.Infrastructure.Keycloak;

/// <summary>
/// Configuration options for integration with Keycloak
/// </summary>
public sealed class KeycloakAdminOptions : KeycloakOptions;

/// <summary>
/// Configuration options for integration with Keycloak
/// </summary>
public sealed class KeycloakClientOptions : KeycloakOptions;

/// <summary>
/// Configuration options for integration with Keycloak
/// </summary>
public class KeycloakOptions
{
    /// <summary>
    /// Keycloak server URL
    /// </summary>
    public required string ServerUrl { get; set; }

    /// <summary>
    /// Keycloak realm name
    /// </summary>
    public required string Realm { get; set; }

    /// <summary>
    /// Keycloak client ID
    /// </summary>
    public required string ClientId { get; set; }

    /// <summary>
    /// Keycloak client secret
    /// </summary>
    public required string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// OpenID Connect metadata URL
    /// </summary>
    public string MetadataUrl => $"{ServerUrl}/realms/{Realm}/.well-known/openid-configuration";

    /// <summary>
    /// Authentication URL
    /// </summary>
    public string AuthorityUrl => $"{ServerUrl}/realms/{Realm}";

    /// <summary>
    /// Keycloak admin API URL
    /// </summary>
    public string AdminUrl => $"{ServerUrl}/admin/realms/{Realm}";

    /// <summary>
    /// Specifies if the connection to Keycloak requires HTTPS
    /// </summary>
    public bool RequireHttps { get; set; } = true;

    /// <summary>
    /// Token expiration time in seconds
    /// </summary>
    public int TokenExpirationSeconds { get; set; } = 300;

    /// <summary>
    /// Maximum timeout for Keycloak API requests in seconds
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Maximum timeout for Keycloak API requests in seconds (alias for TimeoutSeconds)
    /// </summary>
    public int MaxTimeoutSeconds => TimeoutSeconds;

    /// <summary>
    /// Keycloak admin username
    /// </summary>
    public string AdminUsername { get; set; } = string.Empty;

    /// <summary>
    /// Keycloak admin password
    /// </summary>
    public string AdminPassword { get; set; } = string.Empty;

    /// <summary>
    /// Keycloak admin client ID (default: "admin-cli")
    /// </summary>
    public string AdminClientId { get; set; } = "admin-cli";
}
