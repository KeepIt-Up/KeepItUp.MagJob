using Organizations.Application.Features.Invitations.Get;

namespace Organizations.Application.Features.Users.GetUserInvitations;

public sealed record GetUserInvitationsRequest(Guid id) : QueryWithPaginationOptions, IRequest<PaginatedList<Invitation, GetInvitationResponse>>;