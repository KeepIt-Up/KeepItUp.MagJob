using Organizations.Application.Features.Members.Get;

namespace Organizations.Application.Features.Organizations.GetOrganizationMembers;

public sealed class GetOrganizationMembersHandler(
    IOrganizationRepository organizationRepository
    ) : IRequestHandler<GetOrganizationMembersRequest, PaginatedList<Member, GetMemberResponse>>
{
    public async Task<PaginatedList<Member, GetMemberResponse>> Handle(GetOrganizationMembersRequest request, CancellationToken cancellationToken)
    {
        return await organizationRepository.GetMembers<GetMemberResponse>(request.Id, request.Options);
    }
}