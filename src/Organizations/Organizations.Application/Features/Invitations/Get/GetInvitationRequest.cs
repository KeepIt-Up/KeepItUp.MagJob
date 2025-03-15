namespace Organizations.Application.Features.Invitations.Get;

public sealed record GetInvitationRequest(Guid Id) : IRequest<GetInvitationResponse>;