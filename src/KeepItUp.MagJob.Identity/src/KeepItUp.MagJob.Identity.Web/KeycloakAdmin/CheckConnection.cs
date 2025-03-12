using FastEndpoints;
using KeepItUp.MagJob.Identity.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace KeepItUp.MagJob.Identity.Web.KeycloakAdmin;

[HttpGet("api/keycloak/check-connection"), AllowAnonymous]
public class CheckConnection : EndpointWithoutRequest<CheckConnectionResponse>
{
    private readonly KeycloakSettings _settings;
    private readonly HttpClient _httpClient;
    private readonly ILogger<CheckConnection> _logger;

    public CheckConnection(IOptions<KeycloakSettings> settings, IHttpClientFactory httpClientFactory, ILogger<CheckConnection> logger)
    {
        _settings = settings.Value;
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var response = new CheckConnectionResponse
        {
            ServerUrl = _settings.ServerUrl,
            AdminUsername = _settings.AdminUsername,
            AdminClientId = _settings.AdminClientId,
            IsServerReachable = false,
            IsAdminLoginPossible = false,
            ErrorMessage = null
        };

        try
        {
            // Sprawdź, czy serwer jest dostępny
            _logger.LogInformation("Checking if Keycloak server is reachable at {ServerUrl}", _settings.ServerUrl);
            var serverCheckResponse = await _httpClient.GetAsync($"{_settings.ServerUrl}", ct);
            response.IsServerReachable = serverCheckResponse.IsSuccessStatusCode;
            
            if (response.IsServerReachable)
            {
                _logger.LogInformation("Keycloak server is reachable. Checking admin login");
                
                // Sprawdź, czy możemy zalogować się jako admin
                var tokenEndpoint = $"{_settings.ServerUrl}/realms/master/protocol/openid-connect/token";
                var formData = new FormUrlEncodedContent(new System.Collections.Generic.Dictionary<string, string>
                {
                    { "grant_type", "password" },
                    { "client_id", _settings.AdminClientId },
                    { "username", _settings.AdminUsername },
                    { "password", _settings.AdminPassword }
                });
                
                // Dodajemy odpowiedni Content-Type header
                formData.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                
                var tokenResponse = await _httpClient.PostAsync(tokenEndpoint, formData, ct);
                response.IsAdminLoginPossible = tokenResponse.IsSuccessStatusCode;
                
                if (!response.IsAdminLoginPossible)
                {
                    var errorContent = await tokenResponse.Content.ReadAsStringAsync(ct);
                    response.ErrorMessage = $"Admin login failed. Status: {tokenResponse.StatusCode}, Error: {errorContent}";
                    _logger.LogError("Admin login failed. Status: {StatusCode}, Error: {Error}", 
                        tokenResponse.StatusCode, errorContent);
                }
                else
                {
                    var responseContent = await tokenResponse.Content.ReadAsStringAsync(ct);
                    _logger.LogDebug("Token response: {Response}", responseContent);
                    
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = false // Wyłączamy case insensitive, ponieważ używamy JsonPropertyName
                    };
                    
                    var tokenResponseObj = JsonSerializer.Deserialize<KeycloakTokenResponse>(responseContent, options);
                    
                    if (tokenResponseObj != null && !string.IsNullOrEmpty(tokenResponseObj.AccessToken))
                    {
                        _logger.LogInformation("Admin login successful. Access token received.");
                        response.TokenInfo = $"Access token received. Expires in {tokenResponseObj.ExpiresIn} seconds.";
                    }
                    else
                    {
                        _logger.LogWarning("Admin login successful but no access token received.");
                        response.TokenInfo = "Admin login successful but no access token received.";
                    }
                }
            }
            else
            {
                response.ErrorMessage = $"Keycloak server is not reachable. Status: {serverCheckResponse.StatusCode}";
                _logger.LogError("Keycloak server is not reachable. Status: {StatusCode}", serverCheckResponse.StatusCode);
            }
        }
        catch (Exception ex)
        {
            response.ErrorMessage = $"Error checking Keycloak connection: {ex.Message}";
            _logger.LogError(ex, "Error checking Keycloak connection: {Message}", ex.Message);
        }
        
        await SendAsync(response, cancellation: ct);
    }
}

public class CheckConnectionResponse
{
    public string ServerUrl { get; set; } = string.Empty;
    public string AdminUsername { get; set; } = string.Empty;
    public string AdminClientId { get; set; } = string.Empty;
    public bool IsServerReachable { get; set; }
    public bool IsAdminLoginPossible { get; set; }
    public string? TokenInfo { get; set; }
    public string? ErrorMessage { get; set; }
} 
