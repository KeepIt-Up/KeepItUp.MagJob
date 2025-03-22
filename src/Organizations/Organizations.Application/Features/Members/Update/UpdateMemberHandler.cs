using Organizations.Application.Features.Members.Get;

namespace Organizations.Application.Features.Members.Update;

public sealed class UpdateMemberHandler(
    IMemberRepository memberRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper
    ) : IRequestHandler<UpdateMemberRequest, GetMemberResponse>
{
    public async Task<GetMemberResponse> Handle(UpdateMemberRequest request, CancellationToken cancellationToken)
    {
        var member = await memberRepository.Get(request.Id, cancellationToken);
        if (member is null)
        {
            throw new NotFoundException("Member with Id {request.Id} not found");
        }

        member.Update(request.FirstName, request.LastName, request.Notes);

        memberRepository.Update(member);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return mapper.Map<GetMemberResponse>(member);
    }
}