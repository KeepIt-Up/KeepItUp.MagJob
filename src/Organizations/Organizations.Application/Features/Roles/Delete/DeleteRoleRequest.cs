namespace Organizations.Application.Features.Roles.Delete
{
    public sealed record DeleteRoleRequest(Guid Id) : IRequest<Unit>;
}
