using Organizations.Application.Features.Roles.Get;

namespace Organizations.Application.Features.Roles.Update;

public sealed record UpdateRoleRequest : IRequest<GetRoleResponse>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Color { get; set; }
}