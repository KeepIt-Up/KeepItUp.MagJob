using Organizations.Application.Common.Interfaces;

namespace Organizations.Application.Features.Organizations.Create;

public sealed class CreateOrganizationHandler(
    IOrganizationRepository organizationRepository,
    IUnitOfWork unitOfWork,
    IUserAccessor userAccessor,
    IMapper mapper
    ) : IRequestHandler<CreateOrganizationRequest, CreateOrganizationResponse>
{
    public async Task<CreateOrganizationResponse> Handle(CreateOrganizationRequest request, CancellationToken cancellationToken)
    {
        //check if organization name is unique
        var organization = await organizationRepository.GetByName(request.name, cancellationToken);
        if (organization != null)
        {
            throw new ConflictException("Organization name must be unique");
        }

        //create organization
        organization = Organization.Create(request.name, userAccessor.GetUserId(), request.description);
        organizationRepository.Create(organization);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return mapper.Map<CreateOrganizationResponse>(organization);
    }
}