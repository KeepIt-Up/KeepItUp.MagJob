namespace Organizations.Application.Features.Organizations.Get;

public sealed record GetOrganizationResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public Guid OwnerId { get; set; }
    public byte[]? ProfileImage { get; set; }
    public byte[]? BannerImage { get; set; }
}