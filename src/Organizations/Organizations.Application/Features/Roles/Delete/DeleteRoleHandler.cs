namespace Organizations.Application.Features.Roles.Delete
{
    public class DeleteRoleHandler(
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<DeleteRoleRequest, Unit>
    {
        public async Task<Unit> Handle(DeleteRoleRequest request, CancellationToken cancellationToken)
        {
            var role = await roleRepository.Get(request.Id, cancellationToken);
            if (role == null)
            {
                throw new NotFoundException("Role not found");
            }
            roleRepository.Delete(role);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}