namespace Organizations.Application.Features.Organizations.Get;

public sealed class GetOrganizationHandler(
    IOrganizationRepository organizationRepository,
    IMapper mapper
    ) : IRequestHandler<GetOrganizationRequest, GetOrganizationResponse>
{
    public async Task<GetOrganizationResponse> Handle(GetOrganizationRequest request, CancellationToken cancellationToken)
    {
        var organization = await organizationRepository.Get(request.Id, cancellationToken);
        if (organization == null)
        {
            throw new NotFoundException("Organization not found");
        }

        return mapper.Map<GetOrganizationResponse>(organization);
    }
}