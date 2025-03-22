using Organizations.Application.Features.Roles.Get;

namespace Organizations.Application.Features.Roles.Update;

public sealed class UpdateRoleHandler(
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper
    ) : IRequestHandler<UpdateRoleRequest, GetRoleResponse>
{
    public async Task<GetRoleResponse> Handle(UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        var role = await roleRepository.Get(request.Id, cancellationToken);
        if (role is null)
            throw new NotFoundException("Role not found");

        role.Update(request.Name, request.Color);

        roleRepository.Update(role);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return mapper.Map<GetRoleResponse>(role);
    }
}