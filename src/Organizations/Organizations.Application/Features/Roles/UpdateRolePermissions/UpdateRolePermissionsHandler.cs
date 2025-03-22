using Organizations.Application.Features.Roles.Get;

namespace Organizations.Application.Features.Roles.UpdateRolePermissions;

public class UpdateRolePermissionsHandler(
    IRoleRepository roleRepository,
    IPermissionRepository permissionRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper
    ) : IRequestHandler<UpdateRolePermissionsRequest, GetRoleResponse>
{
    public async Task<GetRoleResponse> Handle(UpdateRolePermissionsRequest request, CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetWithPermissions(request.Id, cancellationToken);
        if (role == null)
            throw new NotFoundException("Role not found");

        //get selected permissions
        var selectedPermissions = permissionRepository.GetAll().Where(p => request.PermissionIds.Contains(p.Id)).ToList();

        role.UpdatePermissions(selectedPermissions);

        roleRepository.Update(role);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return mapper.Map<GetRoleResponse>(role);
    }
}
