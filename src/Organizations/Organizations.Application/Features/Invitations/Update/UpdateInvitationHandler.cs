namespace Organizations.Application.Features.Invitations.Update;

public sealed class UpdateInvitationHandler(
    IInvitationRepository invitationRepository
    ) : IRequestHandler<UpdateInvitationRequest, UpdateInvitationResponse>
{
    public async Task<UpdateInvitationResponse> Handle(UpdateInvitationRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}