using Microsoft.EntityFrameworkCore;
using Organizations.Application.Features.Permissions.Get;

namespace Organizations.Application.Features.Permissions.GetPermissions;

public class GetPermissionsHandler(
    IPermissionRepository _permissionRepository,
    IMapper _mapper
    ) : IRequestHandler<GetPermissionsRequest, List<GetPermissionResponse>>
{
    public async Task<List<GetPermissionResponse>> Handle(GetPermissionsRequest request, CancellationToken cancellationToken)
    {
        var permissions = await _permissionRepository.GetAll().ToListAsync(cancellationToken);
        return _mapper.Map<List<GetPermissionResponse>>(permissions);
    }
}