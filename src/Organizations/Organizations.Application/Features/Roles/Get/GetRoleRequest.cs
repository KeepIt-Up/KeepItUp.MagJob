namespace Organizations.Application.Features.Roles.Get;

public sealed record GetRoleRequest : IRequest<GetRoleResponse>
{
    public Guid Id { get; set; }
}