namespace Organizations.Application.Features.Organizations.Get;

public sealed record GetOrganizationRequest(Guid Id) : IRequest<GetOrganizationResponse>;