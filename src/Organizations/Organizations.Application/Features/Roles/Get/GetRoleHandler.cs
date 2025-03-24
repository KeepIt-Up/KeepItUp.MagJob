

namespace Organizations.Application.Features.Roles.Get;

public sealed class GetRoleHandler(
    IRoleRepository roleRepository
    ) : IRequestHandler<GetRoleRequest, GetRoleResponse>
{
    public async Task<GetRoleResponse> Handle(GetRoleRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}