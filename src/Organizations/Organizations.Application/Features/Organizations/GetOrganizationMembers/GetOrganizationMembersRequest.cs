using Organizations.Application.Features.Members.Get;

namespace Organizations.Application.Features.Organizations.GetOrganizationMembers;

public sealed record GetOrganizationMembersRequest(Guid Id) : QueryWithPaginationOptions, IRequest<PaginatedList<Member, GetMemberResponse>>;
