using Organizations.Application.Features.Organizations.Create;
using Organizations.Application.Features.Organizations.CreateOrganizationInvitation;
using Organizations.Application.Features.Organizations.Delete;
using Organizations.Application.Features.Organizations.Get;
using Organizations.Application.Features.Organizations.GetOrganizationInvitations;
using Organizations.Application.Features.Organizations.GetOrganizationMembers;
using Organizations.Application.Features.Organizations.GetOrganizationRoles;
using Organizations.Application.Features.Organizations.Update;

namespace Organizations.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrganizationsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Create a new organization
    /// </summary>
    /// <param name="request"> The request object containing the organization details </param>
    /// <returns> The created organization </returns>
    [HttpPost]
    public async Task<IActionResult> CreateOrganization(CreateOrganizationRequest request)
    {
        return Ok(await mediator.Send(request));
    }

    /// <summary>
    /// Get an organization by its id
    /// </summary>
    /// <param name="request"> The request object containing the organization id </param>
    /// <returns> The organization </returns>
    /// <response code="200"> The organization </response>
    /// <response code="404"> The organization was not found </response>
    /// <response code="401"> The user is not authorized to access this organization </response>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrganizationById(Guid id)
    {
        return Ok(await mediator.Send(new GetOrganizationRequest(id)));
    }

    /// <summary>
    /// Update an organization by its id
    /// </summary>
    /// <param name="request"> The request object containing the organization details </param>
    /// <returns> The updated organization </returns>
    /// <response code="200"> The updated organization </response>
    /// <response code="404"> The organization was not found </response>
    /// <response code="401"> The user is not authorized to access this organization </response>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrganization(Guid id, UpdateOrganizationRequest request)
    {
        return Ok(await mediator.Send(new UpdateOrganizationRequest(id, request.Name, request.Description, request.ProfileImage, request.BannerImage)));
    }

    /// <summary>
    /// Delete an organization by its id
    /// </summary>
    /// <param name="id"> The id of the organization to delete </param>
    /// <returns> No content </returns>
    /// <response code="204"> The organization was deleted </response>
    /// <response code="404"> The organization was not found </response>
    /// <response code="401"> The user is not authorized to access this organization </response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrganization(Guid id)
    {
        await mediator.Send(new DeleteOrganizationRequest(id));
        return NoContent();
    }

    /// <summary>
    /// Create an invitation for an organization
    /// </summary>
    /// <param name="request"> The request object containing the organization id and the user id </param>
    /// <returns> The created invitation </returns>
    /// <response code="200"> The created invitation </response>
    /// <response code="404"> The organization was not found </response>
    /// <response code="401"> The user is not authorized to access this organization </response>
    [HttpPost("{id}/invitations")]
    public async Task<IActionResult> CreateInvitation(Guid organizationId, Guid userId)
    {
        return Ok(await mediator.Send(new CreateOrganizationInvitationRequest(userId, organizationId)));
    }

    /// <summary>
    /// Get invitations by organization id
    /// </summary>
    /// <param name="request"> The request object containing the organization id </param>
    /// <returns> The invitations </returns>
    /// <response code="200"> The invitations </response>
    /// <response code="404"> The organization was not found </response>
    /// <response code="401"> The user is not authorized to access this organization </response>
    [HttpGet("invitations")]
    public async Task<IActionResult> GetInvitationsByOrganizationId([FromQuery] GetOrganizationInvitationsRequest request)
    {
        return Ok(await mediator.Send(request));
    }

    /// <summary>
    /// Get members by organization id
    /// </summary>
    /// <param name="request"> The request object containing the organization id </param>
    /// <returns> The members </returns>
    /// <response code="200"> The members </response>
    /// <response code="404"> The organization was not found </response>
    /// <response code="401"> The user is not authorized to access this organization </response>
    [HttpGet("members")]
    public async Task<IActionResult> GetMembersByOrganizationId([FromQuery] GetOrganizationMembersRequest request)
    {
        return Ok(await mediator.Send(request));
    }

    /// <summary>
    /// Get roles by organization id
    /// </summary>
    /// <param name="request"> The request object containing the organization id </param>
    /// <returns> The roles </returns>
    /// <response code="200"> The roles </response>
    /// <response code="404"> The organization was not found </response>
    /// <response code="401"> The user is not authorized to access this organization </response>
    [HttpGet("roles")]
    public async Task<IActionResult> GetRolesByOrganizationIdPagination([FromQuery] GetOrganizationRolesRequest request)
    {
        return Ok(await mediator.Send(request));
    }
}
