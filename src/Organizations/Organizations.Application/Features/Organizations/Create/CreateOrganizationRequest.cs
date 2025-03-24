namespace Organizations.Application.Features.Organizations.Create;

public sealed record CreateOrganizationRequest(
    string name,
    string? description
) : IRequest<CreateOrganizationResponse>;