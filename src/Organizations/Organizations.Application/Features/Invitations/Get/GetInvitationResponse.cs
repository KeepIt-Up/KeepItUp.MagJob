namespace Organizations.Application.Features.Invitations.Get;

public sealed record GetInvitationResponse(
    Guid Id,
    Guid OrganizationId,
    Guid UserId,
    string OrganizationName,
    string UserName,
    InvitationStatus Status,
    DateTime CreatedAt
);