using KeepItUp.MagJob.Identity.Web.Services;
using Microsoft.AspNetCore.Authorization;

namespace KeepItUp.MagJob.Identity.Web.KeycloakAdmin;

[HttpPost("api/keycloak/users/{UserId}/roles"), AllowAnonymous]
public class AssignRoles : Endpoint<AssignRolesRequest, AssignRolesResponse>
{
  private readonly IKeycloakAdminService _keycloakService;

  public AssignRoles(IKeycloakAdminService keycloakService)
  {
    _keycloakService = keycloakService;
  }

  public override async Task HandleAsync(AssignRolesRequest req, CancellationToken ct)
  {
    var result = await _keycloakService.AssignRolesToUserAsync(req.UserId, req.RoleNames);

    var response = new AssignRolesResponse
    {
      Success = result,
      Message = result ? "Role zostały przypisane pomyślnie" : "Nie udało się przypisać ról"
    };

    await SendAsync(response, cancellation: ct);
  }
}

public class AssignRolesRequest
{
  public string UserId { get; set; } = string.Empty;
  public List<string> RoleNames { get; set; } = new List<string>();
}

public class AssignRolesResponse
{
  public bool Success { get; set; }
  public string Message { get; set; } = string.Empty;
}
