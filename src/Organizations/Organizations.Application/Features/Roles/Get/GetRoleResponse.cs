using Organizations.Application.Features.Members.Get;
using Organizations.Application.Features.Permissions.Get;

namespace Organizations.Application.Features.Roles.Get;

public sealed class GetRoleResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Color { get; set; }
    public Guid OrganizationId { get; set; }
    public List<GetMemberResponse> Members { get; set; } = new List<GetMemberResponse>();
    public List<GetPermissionResponse> Permissions { get; set; } = new List<GetPermissionResponse>();
}