using Organizations.Application.Features.Invitations.Get;

namespace Organizations.Application.Features.Users.GetUserInvitations;

public sealed class GetUserInvitationsHandler(
    IUserRepository userRepository
    ) : IRequestHandler<GetUserInvitationsRequest, PaginatedList<Invitation, GetInvitationResponse>>
{
    public async Task<PaginatedList<Invitation, GetInvitationResponse>> Handle(GetUserInvitationsRequest request, CancellationToken cancellationToken)
    {
        return await userRepository.GetInvitationsByUserIdAsync<GetInvitationResponse>(request.id, request.Options, cancellationToken);
    }
}