using Microsoft.AspNetCore.Mvc;
using Organizations.Application.Features.Roles.AddMembers;
using Organizations.Application.Features.Roles.Create;
using Organizations.Application.Features.Roles.Delete;
using Organizations.Application.Features.Roles.Get;
using Organizations.Application.Features.Roles.RemoveMembers;
using Organizations.Application.Features.Roles.Update;
using Organizations.Application.Features.Roles.UpdateRolePermissions;

namespace Organizations.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Create a role
    /// </summary>
    /// <param name="request"> The request object containing the role name </param>
    /// <returns> The created role </returns>
    /// <response code="200"> The created role </response>
    [HttpPost]
    public async Task<IActionResult> Create(CreateRoleRequest request)
    {
        return Ok(await mediator.Send(request));
    }

    /// <summary>
    /// Get a role by id
    /// </summary>
    /// <param name="request"> The request object containing the role id </param>
    /// <returns> The role </returns>
    /// <response code="200"> The role </response>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(GetRoleRequest request)
    {
        return Ok(await mediator.Send(request));
    }

    /// <summary>
    /// Update a role
    /// </summary>
    /// <param name="request"> The request object containing the role id and the role name </param>
    /// <returns> The updated role </returns>
    /// <response code="200"> The updated role </response>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateRoleRequest request)
    {
        request.Id = id;
        return Ok(await mediator.Send(request));
    }

    /// <summary>
    /// Delete a role
    /// </summary>
    /// <param name="request"> The request object containing the role id </param>
    /// <returns> The deleted role </returns>
    /// <response code="200"> The deleted role </response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await mediator.Send(new DeleteRoleRequest(id));
        return NoContent();
    }

    [HttpPost("{id}/permissions")]
    public async Task<IActionResult> UpdateRolePermissions(Guid id, List<Guid> permissionIds)
    {
        return Ok(await mediator.Send(new UpdateRolePermissionsRequest(id, permissionIds)));
    }

    /// <summary>
    /// Add members to a role
    /// </summary>
    /// <param name="request"> The request object containing the role id and the member ids </param>
    /// <returns> The added members </returns>
    /// <response code="200"> The added members </response>
    [HttpPost("{id}/members")]
    public async Task<IActionResult> AddMembersToRole(Guid id, AddMembersToRoleRequest request)
    {
        request.Id = id;
        return Ok(await mediator.Send(request));
    }

    /// <summary>
    /// Remove members from a role
    /// </summary>
    /// <param name="request"> The request object containing the role id and the member ids </param>
    /// <returns> The removed members </returns>
    /// <response code="200"> The removed members </response>
    [HttpDelete("{id}/members")]
    public async Task<IActionResult> RemoveMembersFromRole(Guid id, RemoveMembersFromRoleRequest request)
    {
        request.Id = id;
        return Ok(await mediator.Send(request));
    }
}