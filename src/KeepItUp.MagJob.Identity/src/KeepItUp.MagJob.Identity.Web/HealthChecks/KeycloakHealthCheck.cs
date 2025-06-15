using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using KeepItUp.MagJob.Identity.Infrastructure.Keycloak;

namespace KeepItUp.MagJob.Identity.Web.HealthChecks;

/// <summary>
/// Health check for Keycloak service availability
/// </summary>
public class KeycloakHealthCheck : IHealthCheck
{
    private readonly HttpClient _httpClient;
    private readonly KeycloakAdminOptions _keycloakOptions;
    private readonly ILogger<KeycloakHealthCheck> _logger;

    /// <summary>
    /// Initializes a new instance of the KeycloakHealthCheck class
    /// </summary>
    /// <param name="httpClient">HTTP client</param>
    /// <param name="keycloakOptions">Keycloak configuration options</param>
    /// <param name="logger">Logger</param>
    public KeycloakHealthCheck(
        HttpClient httpClient,
        IOptions<KeycloakAdminOptions> keycloakOptions,
        ILogger<KeycloakHealthCheck> logger)
    {
        _httpClient = httpClient;
        _keycloakOptions = keycloakOptions.Value;
        _logger = logger;
    }

    /// <summary>
    /// Performs the health check for Keycloak service
    /// </summary>
    /// <param name="context">Health check context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Health check result</returns>
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var realmUrl = $"{_keycloakOptions.ServerUrl}/realms/{_keycloakOptions.Realm}";

            using var response = await _httpClient.GetAsync(realmUrl, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseTime = response.Headers.Date.HasValue
                    ? DateTime.UtcNow - response.Headers.Date.Value.UtcDateTime
                    : TimeSpan.Zero;

                var data = new Dictionary<string, object>
                {
                    ["server_url"] = _keycloakOptions.ServerUrl,
                    ["realm"] = _keycloakOptions.Realm,
                    ["status_code"] = (int)response.StatusCode,
                    ["response_time_ms"] = responseTime.TotalMilliseconds
                };

                _logger.LogDebug("Keycloak health check successful. Realm: {Realm}, Status: {StatusCode}",
                    _keycloakOptions.Realm, response.StatusCode);

                return HealthCheckResult.Healthy("Keycloak is accessible", data);
            }
            else
            {
                var data = new Dictionary<string, object>
                {
                    ["server_url"] = _keycloakOptions.ServerUrl,
                    ["realm"] = _keycloakOptions.Realm,
                    ["status_code"] = (int)response.StatusCode,
                    ["reason"] = response.ReasonPhrase ?? "Unknown error"
                };

                _logger.LogWarning("Keycloak health check failed. Realm: {Realm}, Status: {StatusCode}, Reason: {Reason}",
                    _keycloakOptions.Realm, response.StatusCode, response.ReasonPhrase);

                return HealthCheckResult.Degraded($"Keycloak returned {response.StatusCode}: {response.ReasonPhrase}", null, data);
            }
        }
        catch (HttpRequestException ex)
        {
            var data = new Dictionary<string, object>
            {
                ["server_url"] = _keycloakOptions.ServerUrl,
                ["realm"] = _keycloakOptions.Realm,
                ["error"] = ex.Message
            };

            _logger.LogError(ex, "Keycloak health check failed due to HTTP request exception. Server: {ServerUrl}",
                _keycloakOptions.ServerUrl);

            return HealthCheckResult.Unhealthy("Keycloak is not accessible", ex, data);
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            var data = new Dictionary<string, object>
            {
                ["server_url"] = _keycloakOptions.ServerUrl,
                ["realm"] = _keycloakOptions.Realm,
                ["error"] = "Request timeout"
            };

            _logger.LogError(ex, "Keycloak health check timed out. Server: {ServerUrl}",
                _keycloakOptions.ServerUrl);

            return HealthCheckResult.Degraded("Keycloak request timed out", ex, data);
        }
        catch (Exception ex)
        {
            var data = new Dictionary<string, object>
            {
                ["server_url"] = _keycloakOptions.ServerUrl,
                ["realm"] = _keycloakOptions.Realm,
                ["error"] = ex.Message
            };

            _logger.LogError(ex, "Unexpected error during Keycloak health check. Server: {ServerUrl}",
                _keycloakOptions.ServerUrl);

            return HealthCheckResult.Unhealthy("Unexpected error during Keycloak health check", ex, data);
        }
    }
}