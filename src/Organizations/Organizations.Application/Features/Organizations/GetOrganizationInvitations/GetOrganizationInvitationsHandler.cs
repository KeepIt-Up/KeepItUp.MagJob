using Organizations.Application.Features.Invitations.Get;

namespace Organizations.Application.Features.Organizations.GetOrganizationInvitations;

public sealed class GetOrganizationInvitationsHandler(
    IOrganizationRepository organizationRepository
    ) : IRequestHandler<GetOrganizationInvitationsRequest, PaginatedList<Invitation, GetInvitationResponse>>
{
    public async Task<PaginatedList<Invitation, GetInvitationResponse>> Handle(GetOrganizationInvitationsRequest request, CancellationToken cancellationToken)
    {
        return await organizationRepository.GetInvitations<GetInvitationResponse>(request.Id, request.Options);
    }
}