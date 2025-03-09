using Organizations.Application.Features.Users.Get;
using Organizations.Application.Features.Users.GetUserInvitations;
using Organizations.Application.Features.Users.GetUserOrganizations;

namespace Organizations.API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class UsersController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Get the current identity
    /// </summary>
    /// <returns> The current identity </returns>
    /// <response code="200"> The current identity </response>
    /// <response code="401"> The user is not authenticated </response>
    /// <response code="404"> The user was not found </response>
    [HttpGet("me")]
    public async Task<IActionResult> GetIdentity()
    {
        return Ok(await mediator.Send(new GetUserRequest()));
    }

    /// <summary>
    /// Get the current user's invitations
    /// </summary>
    /// <returns> The current user's invitations </returns>
    /// <response code="200"> The current user's invitations </response>
    /// <response code="401"> The user is not authenticated </response>
    /// <response code="404"> The user was not found </response>
    [HttpGet("invitations")]
    public async Task<IActionResult> GetInvitationsByUserId([FromQuery] GetUserInvitationsRequest request)
    {
        return Ok(await mediator.Send(request));
    }

    /// <summary>
    /// Get the current user's organizations
    /// </summary>
    /// <returns> The current user's organizations </returns>
    /// <response code="200"> The current user's organizations </response>
    /// <response code="401"> The user is not authenticated </response>
    /// <response code="404"> The user was not found </response>
    [HttpGet("organizations")]
    public async Task<IActionResult> GetOrganizationsByUserId([FromQuery] GetUserOrganizationsRequest request)
    {
        return Ok(await mediator.Send(request));
    }
}