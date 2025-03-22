using Organizations.Application.Features.Organizations.Get;

namespace Organizations.Application.Features.Organizations.Update;

public sealed record UpdateOrganizationRequest(
    Guid OrganizationId,
    string? Name,
    string? Description,
    byte[]? ProfileImage,
    byte[]? BannerImage
    ) : IRequest<GetOrganizationResponse>;

