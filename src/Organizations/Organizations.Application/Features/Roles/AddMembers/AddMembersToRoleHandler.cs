namespace Organizations.Application.Features.Roles.AddMembers;

public class AddMembersToRoleHandler(
    IRoleRepository roleRepository,
    IOrganizationRepository organizationRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper
    ) : IRequestHandler<AddMembersToRoleRequest, List<Member>>
{

    public async Task<List<Member>> Handle(AddMembersToRoleRequest request, CancellationToken cancellationToken)
    {
        // check role exists
        var role = await roleRepository.Get(request.Id, cancellationToken);
        if (role == null)
            throw new NotFoundException("Role not found");

        // check organization exists
        var organization = await organizationRepository.Get(role.OrganizationId, cancellationToken);
        if (organization == null)
            throw new NotFoundException("Organization not found");

        // check members exists
        var members = organization.Members.Where(m => request.MemberIds.Contains(m.Id)).ToList();
        if (members.Count != request.MemberIds.Count)
            throw new NotFoundException("Some members not found");

        // add members to role
        role.AddMembers(members);

        // save changes
        roleRepository.Update(role);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // return added members
        return role.Members;
    }

}