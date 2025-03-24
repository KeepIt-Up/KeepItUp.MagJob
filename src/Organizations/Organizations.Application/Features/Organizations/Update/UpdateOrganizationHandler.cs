using Organizations.Application.Features.Organizations.Get;

namespace Organizations.Application.Features.Organizations.Update;

public sealed class UpdateOrganizationHandler(
    IOrganizationRepository organizationRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper
    ) : IRequestHandler<UpdateOrganizationRequest, GetOrganizationResponse>
{
    public async Task<GetOrganizationResponse> Handle(UpdateOrganizationRequest request, CancellationToken cancellationToken)
    {
        var organization = await organizationRepository.Get(request.OrganizationId, cancellationToken);
        if (organization == null)
        {
            throw new NotFoundException("Organization not found");
        }

        if (request.Name != organization.Name && request.Name != null)
        {
            var organizationWithSameName = await organizationRepository.GetByName(request.Name, cancellationToken);
            if (organizationWithSameName != null)
            {
                throw new ConflictException("Organization name must be unique");
            }
        }


        organization.Update(request.Name, request.Description, request.ProfileImage, request.BannerImage);
        organizationRepository.Update(organization);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return mapper.Map<GetOrganizationResponse>(organization);
    }
}