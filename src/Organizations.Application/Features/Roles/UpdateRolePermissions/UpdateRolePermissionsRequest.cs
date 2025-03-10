using Organizations.Application.Features.Roles.Get;

namespace Organizations.Application.Features.Roles.UpdateRolePermissions
{
    public sealed record UpdateRolePermissionsRequest(Guid Id, List<Guid> PermissionIds) : IRequest<GetRoleResponse>;
}
