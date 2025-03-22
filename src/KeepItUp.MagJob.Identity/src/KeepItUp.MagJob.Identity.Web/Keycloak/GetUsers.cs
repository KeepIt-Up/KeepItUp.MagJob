using FastEndpoints;
using KeepItUp.MagJob.Identity.Web.Services;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace KeepItUp.MagJob.Identity.Web.Keycloak;

[HttpGet("api/keycloak/users"), AllowAnonymous]
public class GetUsers : EndpointWithoutRequest<GetUsersResponse>
{
  private readonly IKeycloakAdminService _keycloakService;

  public GetUsers(IKeycloakAdminService keycloakService)
  {
    _keycloakService = keycloakService;
  }

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

public class GetUsersResponse
{
  public IEnumerable<KeycloakUser> Users { get; set; } = new List<KeycloakUser>();
}
