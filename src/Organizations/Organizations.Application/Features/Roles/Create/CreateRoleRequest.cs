using Organizations.Application.Features.Roles.Get;

namespace Organizations.Application.Features.Roles.Create;

public sealed record CreateRoleRequest : IRequest<GetRoleResponse>
{
    public string Name { get; set; } = "New Role";
    public string? Description { get; set; }
    public Guid OrganizationId { get; set; }
    public string? Color { get; set; }
}
