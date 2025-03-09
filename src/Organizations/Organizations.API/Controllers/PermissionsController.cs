using Organizations.Application.Features.Permissions.GetPermissions;

namespace Organizations.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PermissionsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Get all permissions
    /// </summary>
    /// <returns> The permissions </returns>
    /// <response code="200"> The permissions </response>
    /// <response code="401"> The user is not authorized to access this resource </response>
    /// <response code="403"> The user is not authorized to access this resource </response>
    [HttpGet]
    public async Task<IActionResult> GetPermissions()
    {
        return Ok(await mediator.Send(new GetPermissionsRequest()));
    }
}