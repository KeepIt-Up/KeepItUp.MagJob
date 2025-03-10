using Organizations.Application.Features.Invitations.Get;
using Organizations.Application.Features.Invitations.Update;
using Organizations.Domain.Entities;

namespace Organizations.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvitationsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Get an invitation by its id
    /// </summary>
    /// <param name="id"> The id of the invitation </param>
    /// <returns> The invitation </returns>
    /// <response code="200"> The invitation </response>
    /// <response code="404"> The invitation was not found </response>
    /// <response code="401"> The user is not authorized to access this invitation </response>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetInvitationById(Guid id)
    {
        return Ok(await mediator.Send(new GetInvitationRequest(id)));
    }

    /// <summary>
    /// Accept an invitation by its id
    /// </summary>
    /// <param name="id"> The id of the invitation </param>
    /// <returns> The updated invitation </returns>
    /// <response code="200"> The updated invitation </response>
    /// <response code="404"> The invitation was not found </response>
    /// <response code="401"> The user is not authorized to access this invitation </response>
    [HttpPost("{id}/accept")]
    public async Task<IActionResult> AcceptInvitation(Guid id)
    {
        return Ok(await mediator.Send(new UpdateInvitationRequest(id, InvitationStatus.Accepted)));
    }

    /// <summary>
    /// Reject an invitation by its id
    /// </summary>
    /// <param name="id"> The id of the invitation </param>
    /// <returns> The updated invitation </returns>
    /// <response code="200"> The updated invitation </response>
    /// <response code="404"> The invitation was not found </response>
    /// <response code="401"> The user is not authorized to access this invitation </response>
    [HttpPost("{id}/reject")]
    public async Task<IActionResult> RejectInvitation(Guid id)
    {
        return Ok(await mediator.Send(new UpdateInvitationRequest(id, InvitationStatus.Rejected)));
    }

    /// <summary>
    /// Cancel an invitation by its id
    /// </summary>
    /// <param name="id"> The id of the invitation </param>
    /// <returns> The updated invitation </returns>
    /// <response code="200"> The updated invitation </response>
    /// <response code="404"> The invitation was not found </response>
    /// <response code="401"> The user is not authorized to access this invitation </response>
    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelInvitation(Guid id)
    {
        return Ok(await mediator.Send(new UpdateInvitationRequest(id, InvitationStatus.Cancelled)));
    }
}