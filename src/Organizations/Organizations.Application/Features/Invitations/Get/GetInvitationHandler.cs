namespace Organizations.Application.Features.Invitations.Get;

public sealed class GetInvitationHandler(
    IInvitationRepository invitationRepository
    ) : IRequestHandler<GetInvitationRequest, GetInvitationResponse>
{
    public async Task<GetInvitationResponse> Handle(GetInvitationRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
        // var invitation = await _invitationRepository.GetById(request.Id, cancellationToken);
        // return _mapper.Map<GetInvitationResponse>(invitation);
    }
}