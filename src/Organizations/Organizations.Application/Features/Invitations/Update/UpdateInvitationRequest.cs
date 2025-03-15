namespace Organizations.Application.Features.Invitations.Update;

public sealed record UpdateInvitationRequest(
    Guid id,
    InvitationStatus status
) : IRequest<UpdateInvitationResponse>;
