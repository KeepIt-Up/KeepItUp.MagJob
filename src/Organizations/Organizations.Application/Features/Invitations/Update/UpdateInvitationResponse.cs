namespace Organizations.Application.Features.Invitations.Update;

public sealed record UpdateInvitationResponse(
    Guid Id,
    InvitationStatus Status
);