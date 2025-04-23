using KeepItUp.MagJob.Identity.Web.Services;
using Microsoft.AspNetCore.Authorization;

namespace KeepItUp.MagJob.Identity.Web.Keycloak;

/// <summary>
/// Endpoint do pobierania listy użytkowników z Keycloak.
/// </summary>
[HttpGet("api/keycloak/users"), AllowAnonymous]
public class GetUsers : EndpointWithoutRequest<GetUsersResponse>
{
    private readonly IKeycloakAdminService _keycloakService;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="GetUsers"/>.
    /// </summary>
    /// <param name="keycloakService">Serwis administracyjny Keycloak.</param>
    public GetUsers(IKeycloakAdminService keycloakService)
    {
        _keycloakService = keycloakService;
    }

    /// <summary>
    /// Obsługuje żądanie GET /api/keycloak/users.
    /// </summary>
    /// <param name="ct">Token anulowania.</param>
    public override async Task HandleAsync(CancellationToken ct)
    {
        var users = await _keycloakService.GetUsersAsync();

        var response = new GetUsersResponse
        {
            Users = users
        };

        await SendAsync(response, cancellation: ct);
    }
}

/// <summary>
/// Odpowiedź z listą użytkowników Keycloak.
/// </summary>
public class GetUsersResponse
{
    /// <summary>
    /// Lista użytkowników z Keycloak.
    /// </summary>
    public IEnumerable<KeycloakUser> Users { get; set; } = new List<KeycloakUser>();
}
