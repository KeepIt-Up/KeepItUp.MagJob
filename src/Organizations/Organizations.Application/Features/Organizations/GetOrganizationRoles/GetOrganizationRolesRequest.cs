using Organizations.Application.Features.Roles.Get;

namespace Organizations.Application.Features.Organizations.GetOrganizationRoles;

public sealed record GetOrganizationRolesRequest(Guid Id) : QueryWithPaginationOptions, IRequest<PaginatedList<Role, GetRoleResponse>>;
