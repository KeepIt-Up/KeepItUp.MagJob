namespace Organizations.Application.Features.Members.Get;

public sealed class GetMemberHandler(
    IMemberRepository memberRepository
    ) : IRequestHandler<GetMemberRequest, GetMemberResponse>
{
    public async Task<GetMemberResponse> Handle(GetMemberRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}