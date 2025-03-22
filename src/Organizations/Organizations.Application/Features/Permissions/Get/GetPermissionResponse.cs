namespace Organizations.Application.Features.Permissions.Get;

public sealed record GetPermissionResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}
