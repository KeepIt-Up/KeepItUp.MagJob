using KeepItUp.MagJob.Identity.Web.Services;
using Microsoft.AspNetCore.Authorization;

namespace KeepItUp.MagJob.Identity.Web.Keycloak;

[HttpPost("api/keycloak/users"), AllowAnonymous]
public class CreateUser : Endpoint<CreateUserRequest, CreateUserResponse>
{
    private readonly IKeycloakAdminService _keycloakService;

    public CreateUser(IKeycloakAdminService keycloakService)
    {
        _keycloakService = keycloakService;
    }

    public override async Task HandleAsync(CreateUserRequest req, CancellationToken ct)
    {
        var result = await _keycloakService.CreateUserAsync(
            req.Username,
            req.Email,
            req.FirstName,
            req.LastName,
            req.Password,
            req.IsEnabled
        );

        var response = new CreateUserResponse
        {
            Success = result,
            Message = result ? "Użytkownik został utworzony pomyślnie" : "Nie udało się utworzyć użytkownika"
        };

        await SendAsync(response, cancellation: ct);
    }
}

public class CreateUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
}

public class CreateUserResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
