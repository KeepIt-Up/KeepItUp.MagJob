using Organizations.Application.Features.Invitations.Get;

namespace Organizations.Application.Features.Organizations.GetOrganizationInvitations;

public sealed record GetOrganizationInvitationsRequest(Guid Id) : QueryWithPaginationOptions, IRequest<PaginatedList<Invitation, GetInvitationResponse>>;