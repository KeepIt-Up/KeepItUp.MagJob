namespace Organizations.Application.Features.Organizations.Delete;

public sealed record DeleteOrganizationRequest(Guid Id) : IRequest<DeleteOrganizationResponse>;
