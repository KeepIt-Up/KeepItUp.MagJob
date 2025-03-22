using Organizations.Application.Features.Roles.Get;

namespace Organizations.Application.Features.Roles.Create;

public sealed class CreateRoleHandler(
    IOrganizationRepository organizationRepository,
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper
    ) : IRequestHandler<CreateRoleRequest, GetRoleResponse>
{
    public async Task<GetRoleResponse> Handle(CreateRoleRequest request, CancellationToken cancellationToken)
    {
        var organization = await organizationRepository.Get(request.OrganizationId, cancellationToken);
        if (organization is null)
        {
            throw new NotFoundException("Organization not found");
        }

        var role = Role.Create(request.Name, organization.Id, request.Description, request.Color);
        roleRepository.Create(role);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return mapper.Map<GetRoleResponse>(role);
    }
}