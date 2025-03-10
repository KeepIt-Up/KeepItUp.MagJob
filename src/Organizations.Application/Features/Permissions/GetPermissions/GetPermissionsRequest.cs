using Organizations.Application.Features.Permissions.Get;

namespace Organizations.Application.Features.Permissions.GetPermissions;

public sealed record GetPermissionsRequest : IRequest<List<GetPermissionResponse>>;