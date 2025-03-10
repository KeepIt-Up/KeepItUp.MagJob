namespace Organizations.Application.Features.Organizations.CreateOrganizationInvitation;

public sealed class CreateOrganizationInvitationHandler(
    IInvitationRepository invitationRepository
    ) : IRequestHandler<CreateOrganizationInvitationRequest, CreateOrganizationInvitationResponse>
{
    public async Task<CreateOrganizationInvitationResponse> Handle(CreateOrganizationInvitationRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
        // var invitation = await _invitationRepository.Create(request, cancellationToken);
        // return _mapper.Map<CreateInvitationResponse>(invitation);
    }
}