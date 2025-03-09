namespace Organizations.Application.Features.Members.Delete;

public class DeleteMemberHandler(
    IMemberRepository memberRepository
) : IRequestHandler<DeleteMemberRequest, DeleteMemberResponse>
{
    public async Task<DeleteMemberResponse> Handle(DeleteMemberRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}